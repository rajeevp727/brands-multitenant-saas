import api from '../../services/api';

export interface User {
    id: string;
    username: string;
    email: string;
    tenantId: string;
    role?: string;
    name?: string;
    avatarUrl?: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
}

export interface AuthResponse {
    user: User;
}

class AuthService {
    getCurrentUser(): User | null {
        const userStr = localStorage.getItem('saas_user_data');
        if (!userStr) return null;
        try {
            return JSON.parse(userStr);
        } catch {
            return null;
        }
    }

    setCurrentUser(user: User): void {
        localStorage.setItem('saas_user_data', JSON.stringify(user));
    }

    removeCurrentUser(): void {
        localStorage.removeItem('saas_user_data');
        localStorage.removeItem('access_token');
        localStorage.removeItem('user');
    }

    async login(credentials: LoginRequest): Promise<AuthResponse> {
        const response = await api.post<AuthResponse>('/auth/login', credentials);
        const { user } = response.data;
        this.setCurrentUser(user);
        return { user };
    }

    async register(data: RegisterRequest): Promise<AuthResponse> {
        const response = await api.post<AuthResponse>('/auth/register', data);
        const { user } = response.data;
        this.setCurrentUser(user);
        return { user };
    }

    async logout(): Promise<void> {
        try {
            await api.post('/auth/logout');
        } finally {
            this.removeCurrentUser();
            // Optional: Redirect or reload
        }
    }

    async checkAuth(): Promise<User | null> {
        try {
            const response = await api.get<any>('/auth/me');
            const user: User = {
                id: response.data.userId,
                email: response.data.email,
                username: response.data.username,
                tenantId: response.data.tenantId,
                role: response.data.role
            };
            this.setCurrentUser(user);
            return user;
        } catch (error) {
            this.removeCurrentUser();
            return null;
        }
    }
}

export const authService = new AuthService();
export default authService;
