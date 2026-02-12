import { createContext, useContext } from 'react';
import { type User } from '../services/auth';

export interface SavedAccount {
    id: string;
    name: string;
    email: string;
    avatarUrl?: string;
    token: string;
    lastLogin: string;
}

export interface AuthContextType {
    user: User | null;
    login: (email: string, password: string) => Promise<void>;
    logout: () => void;
    isAuthenticated: boolean;
    savedAccounts: SavedAccount[];
    removeAccount: (email: string) => void;
    setSession: (user: User) => void;
    loading: boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within AuthProvider');
    }
    return context;
};
