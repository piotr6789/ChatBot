import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Config } from '../utils/config';

@Injectable({
    providedIn: 'root',
})
export class SignalRService {
    public hubConnection!: signalR.HubConnection;
    private isConnected = false;

    public async startConnection(): Promise<void> {
        if (this.isConnected) return;

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${Config.API_BASE_URL}/chathub`)
            .configureLogging(signalR.LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        try {
            await this.hubConnection.start();
            this.isConnected = true;
            console.log('SignalR connected successfully');
        } catch (err) {
            console.error('SignalR connection error:', err);
            setTimeout(() => this.startConnection(), 3000); // 🔄 Ponowna próba po 3 sekundach
        }

        this.hubConnection.onclose(() => {
            console.warn("SignalR connection lost, attempting to reconnect...");
            this.isConnected = false;
            this.startConnection();
        });
    }

    public async sendMessage(clientSessionId: string, message: string): Promise<void> {
        if (!this.isConnected) {
            console.error("Cannot send message: SignalR is disconnected.");
            return;
        }

        try {
            await this.hubConnection.invoke('SendMessage', clientSessionId, message);
        } catch (err) {
            console.error("Error sending message:", err);
        }
    }

    public cancelMessage(clientSessionId: string): void {
        this.hubConnection.invoke("CancelMessage", clientSessionId)
          .then(() => console.log("Cancellation request sent to server."))
          .catch((err) => console.error("Error canceling message:", err));
    }

    public async stopConnection(): Promise<void> {
        if (!this.isConnected) return;

        try {
            await this.hubConnection.stop();
            this.isConnected = false;
            console.log("SignalR connection stopped.");
        } catch (err) {
            console.error("Error stopping SignalR:", err);
        }
    }
}
