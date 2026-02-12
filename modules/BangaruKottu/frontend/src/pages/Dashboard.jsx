import { useEffect, useState } from 'react';
import Layout from '../components/Layout';
import { dashboardService } from '../services/dashboardService';
import { ShoppingCart, DollarSign, Clock, CheckCircle } from 'lucide-react';

const Dashboard = () => {
  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      const response = await dashboardService.getDashboard();
      setDashboard(response.data);
    } catch (error) {
      console.error('Failed to load dashboard:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      </Layout>
    );
  }

  const cards = [
    {
      title: 'Orders Today',
      value: dashboard?.ordersToday || 0,
      icon: ShoppingCart,
      color: 'bg-blue-500',
      bgColor: 'bg-blue-50',
    },
    {
      title: 'Revenue Today',
      value: `₹${dashboard?.revenueToday?.toFixed(2) || '0.00'}`,
      icon: DollarSign,
      color: 'bg-green-500',
      bgColor: 'bg-green-50',
    },
    {
      title: 'Pending Orders',
      value: dashboard?.pendingOrders || 0,
      icon: Clock,
      color: 'bg-yellow-500',
      bgColor: 'bg-yellow-50',
    },
    {
      title: 'Delivered Orders',
      value: dashboard?.deliveredOrders || 0,
      icon: CheckCircle,
      color: 'bg-purple-500',
      bgColor: 'bg-purple-50',
    },
  ];

  return (
    <Layout>
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-6">Dashboard</h1>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {cards.map((card, index) => {
            const Icon = card.icon;
            return (
              <div
                key={index}
                className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow"
              >
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600 mb-1">{card.title}</p>
                    <p className="text-2xl font-bold text-gray-900">{card.value}</p>
                  </div>
                  <div className={`${card.bgColor} p-3 rounded-lg`}>
                    <Icon className={`${card.color} text-white`} size={24} />
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </Layout>
  );
};

export default Dashboard;

