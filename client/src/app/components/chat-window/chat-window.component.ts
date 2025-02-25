import { Component, OnInit } from '@angular/core';
import { Client, MessageRating, RateMessageDto } from '../../services/chatbot-api';
import { SignalRService } from '../../services/signalr.service';
import { Config } from '../../utils/config';

interface ChatMessage {
  sender: string;
  content: string;
  messageId?: number;
  rating?: number;
}

@Component({
  standalone: false,
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.scss'],
})
export class ChatWindowComponent implements OnInit {
  public message = '';
  public messages: ChatMessage[] = [];
  private apiClient: Client;
  private isSending = false;

  constructor(private signalRService: SignalRService) {
    this.apiClient = new Client(Config.API_BASE_URL);
  }

  ngOnInit(): void {
    this.loadChatHistory();
    this.signalRService.startConnection().then(() => this.registerSignalREvents());
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  private registerSignalREvents(): void {
    const hub = this.signalRService.hubConnection;
    hub.on("ReceiveMessage", this.handleReceiveMessage.bind(this));
    hub.on("ReceiveMessageChunk", this.handleReceiveMessageChunk.bind(this));
    hub.on("CancelMessage", () => this.isSending = false);
    hub.on("MessageSentConfirmation", this.handleMessageSentConfirmation.bind(this));
    hub.on("UpdateMessageId", this.handleUpdateMessageId.bind(this));
  }

  private handleReceiveMessage(sender: string, content: string, messageId: number): void {
    if (!this.messages.some(msg => msg.messageId === messageId)) {
      this.messages.push({ sender: this.getSenderType(sender), content, messageId });
    }
  }

  private handleReceiveMessageChunk(sender: string, partialContent: string, messageId: number): void {
    const existingMessage = this.messages.find(msg => msg.messageId === messageId && msg.sender === "ChatBot");
    existingMessage ? existingMessage.content = partialContent : this.messages.push({ sender: "ChatBot", content: partialContent, messageId });
  }

  private handleMessageSentConfirmation(messageId: number, content: string): void {
    const sentMessage = this.messages.find(msg => msg.content === content && msg.sender === "User" && msg.messageId === undefined);
    sentMessage ? sentMessage.messageId = messageId : this.messages.push({ sender: "User", content, messageId });
  }

  private handleUpdateMessageId(oldMessageId: number, newMessageId: number): void {
    const existingMessage = this.messages.find(msg => msg.messageId === oldMessageId && msg.sender === "ChatBot");
    if (existingMessage) existingMessage.messageId = newMessageId;
    
    this.messages = this.messages.filter(msg => !(msg.messageId === newMessageId && msg.sender === "ChatBot" && msg !== existingMessage));
  }

  private getSessionId(): string {
    let sessionId = localStorage.getItem("ChatSessionId") || crypto.randomUUID();
    localStorage.setItem("ChatSessionId", sessionId);
    return sessionId;
  }

  private getSenderType(sender: string): string {
    return sender.toLowerCase().includes("bot") ? "ChatBot" : "User";
  }

  private scrollToBottom(): void {
    document.querySelector('.chat-messages')?.scrollTo({ top: 99999, behavior: "smooth" });
  }

  loadChatHistory(): void {
    this.apiClient.history(this.getSessionId())
      .then(data => {
        if (!data) return;

        localStorage.setItem("ChatSessionId", data.clientSessionId ?? this.getSessionId());
        this.messages = data.messages?.map(msg => ({
          sender: msg.sender === 0 ? "User" : "ChatBot",
          content: msg.content ?? "",
          messageId: msg.id,
          rating: msg.rating === "Like" ? 1 : msg.rating === "Unlike" ? 0 : undefined
        })) || [];
      })
      .catch(error => console.error('Error loading chat history:', error));
  }

  sendMessage(): void {
    if (!this.message.trim() || this.isSending) return;

    const messageToSend = this.message;
    this.message = '';
    this.messages.push({ sender: 'User', content: messageToSend });

    this.isSending = true;
    this.signalRService.sendMessage(this.getSessionId(), messageToSend)
      .catch(err => console.error("Error sending message:", err))
      .finally(() => this.isSending = false);
  }

  rateMessage(messageId?: number, rating?: number): void {
    if (messageId === undefined || rating === undefined) return;

    const ratingDto = new RateMessageDto();
    ratingDto.messageId = messageId;
    ratingDto.rating = rating as MessageRating;

    this.apiClient.rate(messageId, ratingDto)
      .then(() => {
        const message = this.messages.find(msg => msg.messageId === messageId);
        if (message) message.rating = rating;
      })
      .catch(error => console.error('❌ Error rating message:', error));
  }

  async cancelMessage(): Promise<void> {
    try {
      await this.signalRService.cancelMessage(this.getSessionId());
      this.isSending = false;

      this.incrementLastMessageId();

      this.removeSignalREventListeners();
      await this.restartSignalRConnection();
    } catch (err) {
      console.error("Error canceling message:", err);
    }
  }

  private removeSignalREventListeners(): void {
    const hub = this.signalRService.hubConnection;
    ["ReceiveMessageChunk", "ReceiveMessage", "MessageSentConfirmation", "UpdateMessageId"].forEach(event => hub.off(event));
  }

  private async restartSignalRConnection(): Promise<void> {
    try {
      await this.signalRService.hubConnection.stop();
      console.log("SignalR connection stopped. Restarting...");
      
      await this.signalRService.startConnection();
      this.registerSignalREvents();
      console.log("SignalR connection restored.");
    } catch (err) {
      console.error("Error restarting SignalR:", err);
    }
  }

  private incrementLastMessageId(): void {
    if (this.messages.length > 0) {
      const lastMessage = this.messages[this.messages.length - 1];
      if (lastMessage.messageId !== undefined) lastMessage.messageId += 1;
    }
  }
}
