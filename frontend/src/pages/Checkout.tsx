import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { CheckCircle, CreditCard, Wallet, Banknote, ArrowLeft, ShoppingBag } from 'lucide-react';
import { useCartStore } from '../store/cartStore';
import { useAuthStore } from '../store/authStore';
import { createOrder } from '../api/orders';
import { Button } from '../components/ui/Button';

const paymentMethods = [
  { id: 'credit_card', label: 'Credit Card', icon: CreditCard },
  { id: 'paypal', label: 'PayPal', icon: Wallet },
  { id: 'cash_on_delivery', label: 'Cash on Delivery', icon: Banknote },
];

export function Checkout() {
  const navigate = useNavigate();
  const { items, subtotal, totalDiscount, total, clearCart } = useCartStore();
  const { user } = useAuthStore();
  const [paymentMethod, setPaymentMethod] = useState('credit_card');
  const [success, setSuccess] = useState(false);

  const { mutate: placeOrder, isPending } = useMutation({
    mutationFn: createOrder,
    onSuccess: () => {
      clearCart();
      setSuccess(true);
    },
  });

  if (!user || !user.properties) {
    return (
      <div className="flex min-h-[60vh] flex-col items-center justify-center gap-4 text-center">
        <ShoppingBag className="h-16 w-16 text-indigo-200" />
        <h2 className="text-2xl font-bold text-slate-800">Sign in to checkout</h2>
        <p className="text-slate-500">Please sign in to complete your purchase.</p>
        <Link to="/login">
          <Button size="lg">Sign In</Button>
        </Link>
      </div>
    );
  }

  if (items.length === 0 && !success) {
    navigate('/cart');
    return null;
  }

  if (success) {
    return (
      <div className="flex min-h-[70vh] flex-col items-center justify-center gap-6 text-center">
        <div className="flex h-24 w-24 items-center justify-center rounded-full bg-green-100">
          <CheckCircle className="h-14 w-14 text-green-500" />
        </div>
        <div>
          <h2 className="text-3xl font-bold text-slate-800">Order Placed!</h2>
          <p className="mt-2 text-slate-500">
            Thank you for your purchase. Your order is being processed.
          </p>
        </div>
        <div className="flex gap-3">
          <Link to="/orders">
            <Button variant="secondary">View My Orders</Button>
          </Link>
          <Link to="/products">
            <Button>Continue Shopping</Button>
          </Link>
        </div>
      </div>
    );
  }

  const disc = totalDiscount();
  const sub = subtotal();
  const tot = total();
  const hasDiscount = disc > 0;

  const handleSubmit = () => {
    placeOrder({
      customer: user.user_id,
      total_amount: tot,
      payment_method: paymentMethod,
      discount_applied: hasDiscount,
      items: items.map(i => i.product),
    });
  };

  return (
    <div className="min-h-screen bg-slate-50 py-8">
      <div className="mx-auto max-w-4xl px-4 sm:px-6 lg:px-8">
        <div className="mb-6">
          <Link to="/cart" className="flex items-center gap-1 text-sm text-slate-500 hover:text-indigo-600">
            <ArrowLeft className="h-4 w-4" /> Back to Cart
          </Link>
          <h1 className="mt-3 text-3xl font-bold text-slate-900">Checkout</h1>
        </div>

        <div className="grid gap-8 lg:grid-cols-5">
          {/* Left */}
          <div className="lg:col-span-3 space-y-5">
            {/* Delivery */}
            <div className="rounded-2xl bg-white p-6 shadow-sm">
              <h2 className="mb-4 font-semibold text-slate-800">Delivery Information</h2>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <p className="text-xs text-slate-400 mb-0.5">Name</p>
                  <p className="font-medium text-slate-700">{user.properties.user_name}</p>
                </div>
                <div>
                  <p className="text-xs text-slate-400 mb-0.5">Email</p>
                  <p className="font-medium text-slate-700">{user.email}</p>
                </div>
                <div>
                  <p className="text-xs text-slate-400 mb-0.5">Address</p>
                  <p className="font-medium text-slate-700">{user.properties.location?.address || '—'}</p>
                </div>
                <div>
                  <p className="text-xs text-slate-400 mb-0.5">City</p>
                  <p className="font-medium text-slate-700">{user.properties.location?.city || '—'}</p>
                </div>
                <div>
                  <p className="text-xs text-slate-400 mb-0.5">Country</p>
                  <p className="font-medium text-slate-700">{user.properties.location?.country || '—'}</p>
                </div>
                <div>
                  <p className="text-xs text-slate-400 mb-0.5">ZIP Code</p>
                  <p className="font-medium text-slate-700">{user.properties.location?.zip_code || '—'}</p>
                </div>
              </div>
              <Link to="/profile" className="mt-4 block text-xs text-indigo-600 hover:text-indigo-700">
                Update delivery info →
              </Link>
            </div>

            {/* Payment */}
            <div className="rounded-2xl bg-white p-6 shadow-sm">
              <h2 className="mb-4 font-semibold text-slate-800">Payment Method</h2>
              <div className="space-y-3">
                {paymentMethods.map(({ id, label, icon: Icon }) => (
                  <label
                    key={id}
                    className={`flex cursor-pointer items-center gap-4 rounded-xl border-2 p-4 transition ${
                      paymentMethod === id
                        ? 'border-indigo-500 bg-indigo-50'
                        : 'border-slate-200 hover:border-slate-300'
                    }`}
                  >
                    <input
                      type="radio"
                      name="payment"
                      value={id}
                      checked={paymentMethod === id}
                      onChange={() => setPaymentMethod(id)}
                      className="accent-indigo-600"
                    />
                    <Icon className={`h-5 w-5 ${paymentMethod === id ? 'text-indigo-600' : 'text-slate-400'}`} />
                    <span className={`font-medium ${paymentMethod === id ? 'text-indigo-700' : 'text-slate-700'}`}>
                      {label}
                    </span>
                  </label>
                ))}
              </div>
            </div>
          </div>

          {/* Right - Summary */}
          <div className="lg:col-span-2">
            <div className="sticky top-24 rounded-2xl bg-white p-6 shadow-sm">
              <h2 className="mb-4 font-semibold text-slate-800">Order Summary</h2>
              <div className="mb-4 max-h-48 overflow-y-auto space-y-2">
                {items.map(({ product, quantity }) => (
                  <div key={product.product_id} className="flex justify-between text-sm">
                    <span className="text-slate-600 truncate pr-2">
                      {product.name} ×{quantity}
                    </span>
                    <span className="shrink-0 font-medium text-slate-800">
                      ${(product.price * quantity).toFixed(2)}
                    </span>
                  </div>
                ))}
              </div>
              <div className="border-t border-slate-100 pt-4 space-y-2 text-sm">
                <div className="flex justify-between text-slate-600">
                  <span>Subtotal</span><span>${sub.toFixed(2)}</span>
                </div>
                {disc > 0 && (
                  <div className="flex justify-between text-green-600">
                    <span>Discount</span><span>-${disc.toFixed(2)}</span>
                  </div>
                )}
                <div className="flex justify-between text-slate-600">
                  <span>Shipping</span><span className="text-green-600">Free</span>
                </div>
                <div className="border-t border-slate-100 pt-3 flex justify-between text-base font-bold text-slate-900">
                  <span>Total</span><span>${tot.toFixed(2)}</span>
                </div>
              </div>
              <Button
                onClick={handleSubmit}
                loading={isPending}
                fullWidth
                size="lg"
                className="mt-6"
              >
                Place Order · ${tot.toFixed(2)}
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
