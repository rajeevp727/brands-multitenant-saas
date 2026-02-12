export interface BrandMetric {
    id: string;
    name: string;
    primaryColor: string;
    secondaryColor: string;
    port: number;
    description: string;
}

export interface DashboardStats {
    brandMetrics: BrandMetric[];
    totalRevenue: number;
    ordersToday: number;
    activeVendors: number;
    systemLoad: number;
}
