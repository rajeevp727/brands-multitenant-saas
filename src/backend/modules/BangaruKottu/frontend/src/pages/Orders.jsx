import { useEffect, useState } from 'react';
import Layout from '../components/Layout';
import { orderService } from '../services/orderService';
import { CheckCircle, XCircle, Clock, Truck, Package } from 'lucide-react';

const Orders = () => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadOrders();
  }, []);

  const loadOrders = async () => {
    try {
      const response = await orderService.getAll();
      setOrders(response.data);
    } catch (error) {
      console.error('Failed to load orders:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAction = async (action, orderId) => {
    try {
      await orderService[action](orderId);
      loadOrders();
    } catch (error) {
      alert(error.response?.data?.message || 'Failed to update order');
    }
  };

  const getStatusBadge = (status) => {
    const badges = {
      Pending: 'bg-yellow-100 text-yellow-800',
      Accepted: 'bg-blue-100 text-blue-800',
      Preparing: 'bg-purple-100 text-purple-800',
      Dispatched: 'bg-indigo-100 text-indigo-800',
      Delivered: 'bg-green-100 text-green-800',
      Rejected: 'bg-red-100 text-red-800',
    };
    return badges[status] || 'bg-gray-100 text-gray-800';
  };

  const getActionButtons = (order) => {
    const buttons = [];
    if (order.orderStatus === 'Pending') {
      buttons.push(
        <button
          key="accept"
          onClick={() => handleAction('accept', order.orderId)}
          className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition text-sm font-medium"
        >
          Accept
        </button>,
        <button
          key="reject"
          onClick={() => handleAction('reject', order.orderId)}
          className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition text-sm font-medium"
        >
          Reject
        </button>
      );
    } else if (order.orderStatus === 'Accepted') {
      buttons.push(
        <button
          key="prepare"
          onClick={() => handleAction('prepare', order.orderId)}
          className="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition text-sm font-medium"
        >
          Mark Preparing
        </button>
      );
    } else if (order.orderStatus === 'Preparing') {
      buttons.push(
        <button
          key="dispatch"
          onClick={() => handleAction('dispatch', order.orderId)}
          className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition text-sm font-medium"
        >
          Dispatch
        </button>
      );
    } else if (order.orderStatus === 'Dispatched') {
      buttons.push(
        <button
          key="deliver"
          onClick={() => handleAction('deliver', order.orderId)}
          className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition text-sm font-medium"
        >
          Mark Delivered
        </button>
      );
    }
    return buttons;
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

  return (
    <Layout>
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-6">Orders</h1>
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Order ID
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Customer
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Amount
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Date
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {orders.length === 0 ? (
                  <tr>
                    <td colSpan="6" className="px-6 py-8 text-center text-gray-500">
                      No orders found
                    </td>
                  </tr>
                ) : (
                  orders.map((order) => (
                    <tr key={order.orderId} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                        #{order.orderId}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        <div>
                          <div className="font-medium">{order.customerName || 'N/A'}</div>
                          <div className="text-gray-500">{order.customerPhone || ''}</div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        ₹{order.totalAmount.toFixed(2)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span
                          className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusBadge(
                            order.orderStatus
                          )}`}
                        >
                          {order.orderStatus}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {new Date(order.orderDate).toLocaleDateString()}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                        <div className="flex space-x-2">
                          {getActionButtons(order)}
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Orders;

