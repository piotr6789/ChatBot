import { Component, OnInit } from '@angular/core';
import { Client, CreateMessageDto, RateMessageDto } from '../../services/chatbot-api';
import { SignalRService } from '../../services/signalr.service';

@Component({
  standalone: false,
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.scss'],
})
export class ChatWindowComponent implements OnInit {
  public message: string = '';
  public messages: { sender: string; content: string; messageId?: number }[] = [];
  private apiClient: Client;

  constructor(private signalRService: SignalRService) {
    this.apiClient = new Client('https://localhost:7236');
  }

  ngOnInit(): void {
    this.signalRService.startConnection();

    this.signalRService.hubConnection.on("ReceiveMessage", (sender: string, content: string, messageId: number) => {
      this.messages.push({ sender, content, messageId });
    });

    this.signalRService.hubConnection.on("ReceiveMessageChunk", (sender: string, partialContent: string) => {
      if (this.messages.length > 0 && this.messages[this.messages.length - 1].sender === sender) {
        this.messages[this.messages.length - 1].content = partialContent;
      } else {
        this.messages.push({ sender, content: partialContent });
      }
    });

    this.loadChatHistory();
  }

  ngAfterViewChecked(): void {
    const chatMessagesContainer = document.querySelector('.chat-messages');
    if (chatMessagesContainer) {
      chatMessagesContainer.scrollTop = chatMessagesContainer.scrollHeight;
    }
  }

  loadChatHistory(): void {
    this.apiClient.history()
      .then(() => console.log('Chat history loaded'))
      .catch(error => console.error('Error loading chat history:', error));
  }

  sendMessage(): void {
    if (!this.message.trim()) return;

    const newMessage = new CreateMessageDto();
    newMessage.content = this.message;

    this.apiClient.message(newMessage)
      .then(() => {
        this.signalRService.sendMessage('User', this.message);
        this.messages.push({ sender: 'User', content: this.message });
        this.message = '';
      })
      .catch(error => console.error('Error sending message:', error));
  }

  rateMessage(messageId: number, rating: number): void {
    const ratingDto = new RateMessageDto();
    ratingDto.messageId = messageId;
    ratingDto.rating = rating;

    this.apiClient.rate(messageId, ratingDto)
      .then(() => console.log(`Message ${messageId} rated`))
      .catch(error => console.error('Error rating message:', error));
  }

  cancelMessage(): void {
    this.apiClient.cancel()
      .then(() => console.log('Message generation canceled'))
      .catch(error => console.error('Error canceling message:', error));
  }
}
