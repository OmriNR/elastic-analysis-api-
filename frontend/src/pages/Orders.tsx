import { useQuery } from '@tanstack/react-query';
import { Link, useNavigate } from 'react-router-dom';
import { Package, ChevronRight, Clock } from 'lucide-react';
import { getOrdersByCustomer } from '../api/orders';
import { useAuthStore } from '../store/authStore';
import { PageLoader } from '../components/ui/Loading';
import { Badge } from '../components/ui/Badge';

export function Orders() {
  const { user } = useAuthStore();
  const navigate = useNavigate();

  if (!user) {
    navigate('/login');
    return null;
  }

  const { data: orders = [], isLoading } = useQuery({
    queryKey: ['orders', user.user_id],
    queryFn: () => getOrdersByCustomer(user.user_id),
  });

  if (isLoading) return <PageLoader />;

  return (
    <div className="min-h-screen bg-slate-50 py-8">
      <div className="mx-auto max-w-3xl px-4 sm:px-6 lg:px-8">
        <h1 className="mb-6 text-3xl font-bold text-slate-900">My Orders</h1>

        {orders.length === 0 ? (
          <div className="flex flex-col items-center justify-center rounded-2xl bg-white py-20 text-center shadow-sm">
            <Package className="mb-4 h-16 w-16 text-indigo-200" />
            <h2 className="text-xl font-semibold text-slate-700">No orders yet</h2>
            <p className="mt-1 text-sm text-slate-400">Start shopping to see your orders here.</p>
            <Link to="/products" className="mt-6">
              <button className="rounded-xl bg-indigo-600 px-6 py-2.5 text-sm font-medium text-white hover:bg-indigo-700">
                Browse Products
              </button>
            </Link>
          </div>
        ) : (
          <div className="space-y-4">
            {orders.map(order => (
              <div key={order.order_id} className="rounded-2xl bg-white p-5 shadow-sm">
                <div className="flex items-start justify-between">
                  <div>
                    <p className="font-semibold text-slate-800">Order #{order.order_id.slice(0, 8)}</p>
                    <div className="mt-1 flex items-center gap-1.5 text-xs text-slate-400">
                      <Clock className="h-3.5 w-3.5" />
                      {new Date(order.timestamp).toLocaleDateString('en-US', {
                        year: 'numeric', month: 'long', day: 'numeric',
                      })}
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="text-lg font-bold text-slate-800">${order.total_amount.toFixed(2)}</p>
                    {order.discount_applied && <Badge variant="green">Discount applied</Badge>}
                  </div>
                </div>

                <div className="mt-4 border-t border-slate-100 pt-4">
                  <p className="mb-2 text-xs font-medium text-slate-500 uppercase tracking-wider">Items</p>
                  <div className="space-y-1.5">
                    {order.items.map(item => (
                      <div key={item.product_id} className="flex items-center justify-between text-sm">
                        <Link
                          to={`/products/${item.product_id}`}
                          className="flex items-center gap-2 text-slate-700 hover:text-indigo-600 transition-colors"
                        >
                          <ChevronRight className="h-3.5 w-3.5 text-slate-300" />
                          {item.name}
                        </Link>
                        <span className="font-medium text-slate-600">${item.price.toFixed(2)}</span>
                      </div>
                    ))}
                  </div>
                </div>

                <div className="mt-3 flex items-center justify-between text-xs text-slate-400">
                  <span>Payment: {order.payment_method.replace(/_/g, ' ')}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
