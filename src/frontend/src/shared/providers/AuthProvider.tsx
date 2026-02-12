import React, { useState, useEffect, useCallback, type ReactNode } from 'react';
import authService, { type User } from '../../shared/services/auth';
import { AuthContext } from './AuthContext';
import type { SavedAccount } from './AuthContext';

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<User | null>(authService.getCurrentUser());
    const [loading, setLoading] = useState(true);
    const [savedAccounts, setSavedAccounts] = useState<SavedAccount[]>(() => {
        const storedAccounts = localStorage.getItem('saved_accounts');
        if (storedAccounts) {
            try {
                return JSON.parse(storedAccounts);
            } catch (e) {
                console.error('Failed to parse saved accounts', e);
            }
        }
        return [];
    });

    const logout = useCallback(async () => {
        await authService.logout();
        setUser(null);
    }, []);

    useEffect(() => {
        const initAuth = async () => {
            const currentUser = await authService.checkAuth();
            setUser(currentUser);
            setLoading(false);
        };
        initAuth();
    }, []);

    const saveAccount = useCallback((user: User) => {
        const newAccount: SavedAccount = {
            id: user.id || user.email,
            name: user.username || user.email.split('@')[0],
            email: user.email,
            avatarUrl: user.avatarUrl,
            token: '', // Tokens are now HttpOnly cookies, no longer saved in localStorage
            lastLogin: new Date().toISOString()
        };

        setSavedAccounts(prev => {
            const filtered = prev.filter(a => a.email !== user.email);
            const updated = [newAccount, ...filtered];
            localStorage.setItem('saved_accounts', JSON.stringify(updated));
            return updated;
        });
    }, []);

    const removeAccount = useCallback((email: string) => {
        setSavedAccounts(prev => {
            const updated = prev.filter(a => a.email !== email);
            localStorage.setItem('saved_accounts', JSON.stringify(updated));
            return updated;
        });
    }, []);

    const login = useCallback(async (email: string, password: string) => {
        const response = await authService.login({ email, password });
        setUser(response.user);
        saveAccount(response.user);
    }, [saveAccount]);

    const setSession = useCallback((user: User) => {
        authService.setCurrentUser(user);
        setUser(user);
        saveAccount(user);
    }, [saveAccount]);

    return (
        <AuthContext.Provider value={{ user, login, logout, isAuthenticated: !!user, savedAccounts, removeAccount, setSession, loading }}>
            {children}
        </AuthContext.Provider>
    );
};
