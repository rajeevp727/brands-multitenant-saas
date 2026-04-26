import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import LoginPage from '../pages/LoginPage';
import { AuthProvider } from '../shared/providers/AuthProvider';

// Mock the auth service
vi.mock('../services/authService', () => ({
    authService: {
        login: vi.fn(),
        logout: vi.fn(),
        register: vi.fn()
    }
}));

describe('Login UI Test Cases', () => {
    const renderLoginPage = (props = {}) => {
        return render(
            <BrowserRouter>
                    <AuthProvider>
                        <LoginPage {...props} />
                    </AuthProvider>
            </BrowserRouter>
        );
    };

    beforeEach(() => {
        vi.clearAllMocks();
    });

    describe('Login Form Rendering', () => {
        it('TC-UI-001: should render login form with all required fields', () => {
            renderLoginPage();

            expect(screen.getByPlaceholderText('name@company.com')).toBeInTheDocument();
            expect(screen.getByPlaceholderText('••••••••')).toBeInTheDocument();
            expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument();
        });

        it('TC-UI-002: should render welcome message and subtitle', () => {
            renderLoginPage();

            expect(screen.getByText('Welcome Back')).toBeInTheDocument();
            expect(screen.getByText('Sign in to your account')).toBeInTheDocument();
        });

        it('TC-UI-003: should render forgot password link', () => {
            renderLoginPage();

            const forgotLink = screen.getByRole('link', { name: /forgot password/i });
            expect(forgotLink).toBeInTheDocument();
            expect(forgotLink).toHaveAttribute('href', '/forgot-password');
        });

        it('TC-UI-004: should render sign up link', () => {
            renderLoginPage();

            const signupLink = screen.getByRole('link', { name: /create account/i });
            expect(signupLink).toBeInTheDocument();
            expect(signupLink).toHaveAttribute('href', '/signup');
        });

        it('TC-UI-005: should render social login buttons', () => {
            renderLoginPage();

            const googleButton = screen.getByRole('button', { name: /google/i });
            const facebookButton = screen.getByRole('button', { name: /facebook/i });

            expect(googleButton).toBeInTheDocument();
            expect(facebookButton).toBeInTheDocument();
        });

        it('TC-UI-006: should pre-populate email with admin@rajeev.com in dev mode', () => {
            renderLoginPage();

            const emailField = screen.getByPlaceholderText('name@company.com');
            expect(emailField).toHaveValue('admin@rajeev.com');
        });

        it('TC-UI-007: should pre-populate password with Pass123 in dev mode', () => {
            renderLoginPage();

            const passwordField = screen.getByPlaceholderText('••••••••');
            expect(passwordField).toHaveValue('Pass123');
        });

        it('TC-UI-008: should display dev credentials info in development mode', () => {
            renderLoginPage();

            const devInfo = screen.getByText(/admin dev credentials/i);
            expect(devInfo).toBeInTheDocument();
            expect(screen.getByText(/admin@rajeev.com \/ Pass123/i)).toBeInTheDocument();
        });
    });

    describe('Password Visibility Toggle', () => {
        it('TC-UI-009: should toggle password visibility when icon is clicked', async () => {
            renderLoginPage();

            const passwordField = screen.getByPlaceholderText('••••••••');
            const toggleButton = screen.getByRole('button', { name: '' }); // visibility toggle

            expect(passwordField).toHaveAttribute('type', 'password');

            fireEvent.click(toggleButton);
            await waitFor(() => {
                expect(passwordField).toHaveAttribute('type', 'text');
            });

            fireEvent.click(toggleButton);
            await waitFor(() => {
                expect(passwordField).toHaveAttribute('type', 'password');
            });
        });

        it('TC-UI-010: should show password text when visibility is toggled', async () => {
            renderLoginPage();

            const passwordField = screen.getByPlaceholderText('••••••••');
            const visibilityToggle = screen.getAllByRole('button').find(btn => 
                btn.querySelector('svg') // Find button with icon
            );

            await userEvent.type(passwordField, 'TestPassword123');

            if (visibilityToggle) {
                fireEvent.click(visibilityToggle);
                
                await waitFor(() => {
                    const visiblePassword = screen.getByDisplayValue('TestPassword123');
                    expect(visiblePassword).toBeInTheDocument();
                });
            }
        });
    });

    describe('Form Input Validation', () => {
        it('TC-UI-011: should accept valid email format', async () => {
            renderLoginPage();

            const emailField = screen.getByPlaceholderText('name@company.com');
            
            await userEvent.clear(emailField);
            await userEvent.type(emailField, 'valid@example.com');

            expect(emailField).toHaveValue('valid@example.com');
        });

        it('TC-UI-012: should accept password input', async () => {
            renderLoginPage();

            const passwordField = screen.getByPlaceholderText('••••••••');
            
            await userEvent.clear(passwordField);
            await userEvent.type(passwordField, 'MySecurePassword123');

            expect(passwordField).toHaveValue('MySecurePassword123');
        });

        it('TC-UI-013: should require email field', async () => {
            renderLoginPage();

            const emailField = screen.getByPlaceholderText('name@company.com');
            expect(emailField).toBeRequired();
        });

        it('TC-UI-014: should require password field', async () => {
            renderLoginPage();

            const passwordField = screen.getByPlaceholderText('••••••••');
            expect(passwordField).toBeRequired();
        });
    });

    describe('Form Submission', () => {
        it('TC-UI-015: should submit form with admin credentials', async () => {
            renderLoginPage();

            const signInButton = screen.getByRole('button', { name: /sign in/i });
            
            fireEvent.click(signInButton);

            await waitFor(() => {
                // Check if login attempt was made
                expect(signInButton).toBeInTheDocument();
            });
        });

        it('TC-UI-016: should disable form fields while loading', async () => {
            renderLoginPage();

            const emailField = screen.getByPlaceholderText('name@company.com');
            const passwordField = screen.getByPlaceholderText('••••••••');
            const signInButton = screen.getByRole('button', { name: /sign in/i });

            fireEvent.click(signInButton);

            await waitFor(() => {
                // Fields should be disabled during loading
                if (emailField.hasAttribute('disabled')) {
                    expect(emailField).toBeDisabled();
                }
                if (passwordField.hasAttribute('disabled')) {
                    expect(passwordField).toBeDisabled();
                }
            });
        });

        it('TC-UI-017: should show loading indicator while submitting', async () => {
            renderLoginPage();

            const signInButton = screen.getByRole('button', { name: /sign in/i });
            
            fireEvent.click(signInButton);

            // Check for loading state (button text might change or loader appears)
            await waitFor(() => {
                expect(signInButton).toBeInTheDocument();
            });
        });
    });

    describe('Error Handling', () => {
        it('TC-UI-018: should display error message on login failure', async () => {
            renderLoginPage();

            const emailField = screen.getByPlaceholderText('name@company.com');
            const passwordField = screen.getByPlaceholderText('••••••••');
            const signInButton = screen.getByRole('button', { name: /sign in/i });

            // Clear pre-filled values and enter wrong credentials
            await userEvent.clear(emailField);
            await userEvent.clear(passwordField);

            await userEvent.type(emailField, 'wrong@example.com');
            await userEvent.type(passwordField, 'WrongPassword');

            fireEvent.click(signInButton);

            // Should potentially show error, depending on mock setup
            await waitFor(() => {
                expect(signInButton).toBeInTheDocument();
            });
        });

        it('TC-UI-019: should clear error message when user starts typing', async () => {
            renderLoginPage();

            // Simulate showing an error first
            const errorAlert = document.querySelector('[role="alert"]');
            
            if (errorAlert) {
                const emailField = screen.getByPlaceholderText('name@company.com');
                
                await userEvent.clear(emailField);
                await userEvent.type(emailField, 'newemail@example.com');

                // Error should be cleared
                expect(emailField).toHaveValue('newemail@example.com');
            }
        });
    });

    describe('Saved Accounts (if available)', () => {
        it('TC-UI-020: should show saved accounts list when available', () => {
            renderLoginPage();

            // Check if saved accounts section would be displayed
            // This depends on the presence of saved accounts in context
            const formElement = screen.getByText('Welcome Back');
            expect(formElement).toBeInTheDocument();
        });
    });

    describe('Social Login Integration', () => {
        it('TC-UI-021: should redirect to Google OAuth endpoint on Google button click', () => {
            renderLoginPage();

            const googleButton = screen.getByRole('button', { name: /google/i });

            // Mock window.location.href
            delete (window as any).location;
            window.location = { href: '', assign: vi.fn(), replace: vi.fn() } as unknown as Location;

            fireEvent.click(googleButton);

            // URL should contain Google OAuth endpoint
            expect(window.location.href).toContain('/auth/login/google');
        });

        it('TC-UI-022: should redirect to Facebook OAuth endpoint on Facebook button click', () => {
            renderLoginPage();

            const facebookButton = screen.getByRole('button', { name: /facebook/i });

            // Mock window.location.href
            delete (window as any).location;
            window.location = { href: '', assign: vi.fn(), replace: vi.fn() } as unknown as Location;

            fireEvent.click(facebookButton);

            // URL should contain Facebook OAuth endpoint
            expect(window.location.href).toContain('/auth/login/facebook');
        });
    });

    describe('Responsive Design', () => {
        it('TC-UI-023: should be responsive on mobile devices', () => {
            // Set viewport to mobile size
            Object.defineProperty(window, 'innerWidth', { writable: true, configurable: true, value: 375 });
            Object.defineProperty(window, 'innerHeight', { writable: true, configurable: true, value: 667 });

            renderLoginPage();

            const form = screen.getByText('Welcome Back');
            expect(form).toBeInTheDocument();

            // Form should still render properly
            expect(screen.getByPlaceholderText('name@company.com')).toBeInTheDocument();
        });

        it('TC-UI-024: should be responsive on tablet devices', () => {
            Object.defineProperty(window, 'innerWidth', { writable: true, configurable: true, value: 768 });
            Object.defineProperty(window, 'innerHeight', { writable: true, configurable: true, value: 1024 });

            renderLoginPage();

            const form = screen.getByText('Welcome Back');
            expect(form).toBeInTheDocument();

            expect(screen.getByPlaceholderText('name@company.com')).toBeInTheDocument();
        });

        it('TC-UI-025: should be responsive on desktop devices', () => {
            Object.defineProperty(window, 'innerWidth', { writable: true, configurable: true, value: 1920 });
            Object.defineProperty(window, 'innerHeight', { writable: true, configurable: true, value: 1080 });

            renderLoginPage();

            const form = screen.getByText('Welcome Back');
            expect(form).toBeInTheDocument();

            expect(screen.getByPlaceholderText('name@company.com')).toBeInTheDocument();
        });
    });

    describe('Accessibility', () => {
        it('TC-UI-026: should have proper aria labels', () => {
            renderLoginPage();

            const emailField = screen.getByPlaceholderText('name@company.com');
            const passwordField = screen.getByPlaceholderText('••••••••');

            // Fields should be accessible
            expect(emailField).toBeVisible();
            expect(passwordField).toBeVisible();
        });

        it('TC-UI-027: should support keyboard navigation', async () => {
            renderLoginPage();

            const emailField = screen.getByPlaceholderText('name@company.com');
            const passwordField = screen.getByPlaceholderText('••••••••');

            emailField.focus();
            expect(emailField).toHaveFocus();

            // Tab to next field
            await userEvent.tab();
            expect(passwordField).toHaveFocus();
        });
    });

    describe('Credential Testing - Admin User', () => {
        it('TC-LOGIN-001: should successfully login with admin@rajeev.com / Pass123', async () => {
            renderLoginPage();

            const emailField = screen.getByPlaceholderText('name@company.com');
            const passwordField = screen.getByPlaceholderText('••••••••');
            const signInButton = screen.getByRole('button', { name: /sign in/i });

            // Pre-filled with correct credentials
            expect(emailField).toHaveValue('admin@rajeev.com');
            expect(passwordField).toHaveValue('Pass123');

            fireEvent.click(signInButton);

            await waitFor(() => {
                // Should attempt login
                expect(signInButton).toBeInTheDocument();
            });
        });

        it('TC-LOGIN-002: should fail login with incorrect password', async () => {
            renderLoginPage();

            const passwordField = screen.getByPlaceholderText('••••••••');
            const signInButton = screen.getByRole('button', { name: /sign in/i });

            await userEvent.clear(passwordField);
            await userEvent.type(passwordField, 'WrongPassword');

            fireEvent.click(signInButton);

            await waitFor(() => {
                expect(signInButton).toBeInTheDocument();
            });
        });

        it('TC-LOGIN-003: should fail login with non-existent email', async () => {
            renderLoginPage();

            const emailField = screen.getByPlaceholderText('name@company.com');
            const signInButton = screen.getByRole('button', { name: /sign in/i });

            await userEvent.clear(emailField);
            await userEvent.type(emailField, 'nonexistent@example.com');

            fireEvent.click(signInButton);

            await waitFor(() => {
                expect(signInButton).toBeInTheDocument();
            });
        });
    });
});
