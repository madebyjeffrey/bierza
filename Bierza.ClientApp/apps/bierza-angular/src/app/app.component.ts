import { Component } from '@angular/core';
import { UserService } from './services/user.service';

@Component({
    selector: 'bierza-client-app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent {
    title = 'bierza-angular';

    constructor(protected userService: UserService) {}

    protected handleLogout() {
        this.userService.logout();
    }

    protected handleLogin() {
        this.userService.login('joe', 'password');
    }
}
