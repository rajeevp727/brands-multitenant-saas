
import { render, screen, waitFor } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import Dashboard from '../pages/Dashboard';
import { AuthProvider } from '../shared/providers/AuthProvider';
import api from '../shared/services/api';

// Mock the API
vi.mock('../shared/services/api');

const mockBrands = [
  {
    id: '1',
    tenantId: 'greenpantry',
    name: 'GreenPantry',
    slogan: 'Organic Food',
    description: 'Fresh organic produce',
    logoUrl: '/green.png',
    primaryColor: '#10b981',
    secondaryColor: '#ffffff',
    configJson: '{"url": "https://greenpantry.in"}',
    isVisible: true,
    sortOrder: 1
  },
  {
    id: '2',
    tenantId: 'omega',
    name: 'Omega Technologies',
    slogan: 'Digital Solutions',
    description: 'Enterprise software',
    logoUrl: '/omega.png',
    primaryColor: '#3b82f6',
    secondaryColor: '#1e293b',
    configJson: '{"url": "https://omega-technologies.in"}',
    isVisible: true,
    sortOrder: 2
  }
];

describe('Dashboard Component', () => {
    beforeEach(() => {
        vi.clearAllMocks();
        (api.get as any).mockResolvedValue({ data: mockBrands });
    });

    const renderDashboard = () => {
        return render(
            <BrowserRouter>
                <AuthProvider>
                    <Dashboard />
                </AuthProvider>
            </BrowserRouter>
        );
    };

    it('should render the hero section', () => {
        renderDashboard();
        expect(screen.getByText(/Multi-brand dashboard/i)).toBeInTheDocument();
    });

    it('should fetch and display brands', async () => {
        renderDashboard();

        await waitFor(() => {
            expect(api.get).toHaveBeenCalledWith('/brands');
            expect(screen.getByText('GreenPantry')).toBeInTheDocument();
            expect(screen.getByText('Omega Technologies')).toBeInTheDocument();
        });
    });

    it('should not display rajeev-pvt (corporate) brand in the grid', async () => {
        const brandsWithCorporate = [
            ...mockBrands,
            {
                id: '0',
                tenantId: 'rajeev-pvt',
                name: "Rajeev's Tech",
                isVisible: true,
                sortOrder: 0
            }
        ];
        (api.get as any).mockResolvedValue({ data: brandsWithCorporate });

        renderDashboard();

        await waitFor(() => {
            expect(screen.queryByText("Rajeev's Tech")).not.toBeInTheDocument();
        });
    });
});
