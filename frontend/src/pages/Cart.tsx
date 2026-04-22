import { Link, useNavigate } from 'react-router-dom';
import { Minus, Plus, Trash2, ShoppingBag, ArrowLeft, ArrowRight } from 'lucide-react';
import { useCartStore } from '../store/cartStore';
import { productImageUrl } from '../api/client';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';

export function Cart() {
  const { items, updateQty, removeItem, clearCart, subtotal, totalDiscount, total } = useCartStore();
  const navigate = useNavigate();

  if (items.length === 0) {
    return (
      <div className="flex min-h-[70vh] flex-col items-center justify-center gap-6 text-center">
        <div className="flex h-24 w-24 items-center justify-center rounded-3xl bg-indigo-50">
          <ShoppingBag className="h-12 w-12 text-indigo-300" />
        </div>
        <div>
          <h2 className="text-2xl font-bold text-slate-800">Your cart is empty</h2>
          <p className="mt-1 text-slate-500">Add some products and they'll appear here.</p>
        </div>
        <Link to="/products">
          <Button size="lg">Start Shopping <ArrowRight className="h-5 w-5" /></Button>
        </Link>
      </div>
    );
  }

  const disc = totalDiscount();
  const sub = subtotal();
  const tot = total();

  return (
    <div className="min-h-screen bg-slate-50 py-8">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-3xl font-bold text-slate-900">Shopping Cart</h1>
          <button
            onClick={clearCart}
            className="text-sm text-red-500 transition hover:text-red-700"
          >
            Clear all
          </button>
        </div>

        <div className="grid gap-8 lg:grid-cols-3">
          {/* Items */}
          <div className="lg:col-span-2 space-y-4">
            {items.map(({ product, quantity, discount }) => {
              const isDiscountValid = discount && new Date(discount.expiredAt) > new Date();
              const itemPrice = isDiscountValid
                ? product.price * (1 - discount.percentage / 100)
                : product.price;

              return (
                <div key={product.productId} className="flex gap-4 rounded-2xl bg-white p-4 shadow-sm">
                  <div className="h-24 w-24 shrink-0 overflow-hidden rounded-xl bg-slate-100">
                    <img
                      src={productImageUrl(product.productId)}
                      alt={product.name}
                      className="h-full w-full object-cover"
                      onError={e => {
                        const el = e.currentTarget;
                        el.style.display = 'none';
                      }}
                    />
                  </div>

                  <div className="flex flex-1 flex-col justify-between">
                    <div>
                      <div className="flex items-start justify-between">
                        <div>
                          <Link
                            to={`/products/${product.productId}`}
                            className="font-semibold text-slate-800 hover:text-indigo-600 transition-colors"
                          >
                            {product.name}
                          </Link>
                          <div className="mt-1 flex gap-1.5">
                            <Badge variant="indigo">{product.category}</Badge>
                            {isDiscountValid && (
                              <Badge variant="red">-{discount.percentage}%</Badge>
                            )}
                          </div>
                        </div>
                        <button
                          onClick={() => removeItem(product.productId)}
                          className="text-slate-400 transition hover:text-red-500"
                        >
                          <Trash2 className="h-4 w-4" />
                        </button>
                      </div>
                    </div>

                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2 rounded-xl border border-slate-200 p-1">
                        <button
                          onClick={() => updateQty(product.productId, quantity - 1)}
                          className="flex h-7 w-7 items-center justify-center rounded-lg text-slate-600 transition hover:bg-slate-100"
                        >
                          <Minus className="h-3.5 w-3.5" />
                        </button>
                        <span className="w-8 text-center text-sm font-semibold">{quantity}</span>
                        <button
                          onClick={() => updateQty(product.productId, Math.min(product.quantity, quantity + 1))}
                          className="flex h-7 w-7 items-center justify-center rounded-lg text-slate-600 transition hover:bg-slate-100"
                        >
                          <Plus className="h-3.5 w-3.5" />
                        </button>
                      </div>
                      <div className="text-right">
                        <p className="font-bold text-slate-800">${(itemPrice * quantity).toFixed(2)}</p>
                        {isDiscountValid && (
                          <p className="text-xs text-slate-400 line-through">
                            ${(product.price * quantity).toFixed(2)}
                          </p>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>

          {/* Summary */}
          <div className="lg:col-span-1">
            <div className="sticky top-24 rounded-2xl bg-white p-6 shadow-sm">
              <h2 className="mb-5 text-lg font-bold text-slate-800">Order Summary</h2>
              <div className="space-y-3 text-sm">
                <div className="flex justify-between text-slate-600">
                  <span>Subtotal</span>
                  <span>${sub.toFixed(2)}</span>
                </div>
                {disc > 0 && (
                  <div className="flex justify-between text-green-600">
                    <span>Discounts</span>
                    <span>-${disc.toFixed(2)}</span>
                  </div>
                )}
                <div className="flex justify-between text-slate-600">
                  <span>Shipping</span>
                  <span className="text-green-600">Free</span>
                </div>
                <div className="border-t border-slate-100 pt-3 flex justify-between text-base font-bold text-slate-900">
                  <span>Total</span>
                  <span>${tot.toFixed(2)}</span>
                </div>
              </div>

              <Button
                onClick={() => navigate('/checkout')}
                fullWidth
                size="lg"
                className="mt-6"
              >
                Proceed to Checkout <ArrowRight className="h-5 w-5" />
              </Button>

              <Link
                to="/products"
                className="mt-4 flex items-center justify-center gap-1 text-sm text-slate-500 transition hover:text-indigo-600"
              >
                <ArrowLeft className="h-4 w-4" /> Continue Shopping
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
