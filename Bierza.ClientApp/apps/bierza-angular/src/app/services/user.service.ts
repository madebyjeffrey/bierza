import { Injectable } from '@angular/core';
import { BierzaClient } from '../clients/bierza-client';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    private loggedIn = new BehaviorSubject(false);

    constructor(private apiClient: BierzaClient) {}

    isLoggedIn(): Observable<boolean> {
        return this.loggedIn;
    }

    logout() {
        this.loggedIn.next(false);
    }

    login(username: string, password: string) {
        this.loggedIn.next(true);
    }
}
