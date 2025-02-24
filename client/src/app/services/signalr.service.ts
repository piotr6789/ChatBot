import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Config } from '../utils/config';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  public hubConnection!: signalR.HubConnection;

  public startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${Config.API_BASE_URL}/chathub`)
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch((err) => console.error('SignalR error:', err));
  }

  public sendMessage(user: string, message: string): void {
    this.hubConnection.invoke('SendMessage', user, message).catch((err) => console.error(err));
  }
}
