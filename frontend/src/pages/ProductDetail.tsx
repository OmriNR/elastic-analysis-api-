import { useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { ShoppingCart, ArrowLeft, Tag, Package, ChevronRight, Minus, Plus } from 'lucide-react';
import { getProduct } from '../api/products';
import { getDiscountByProduct } from '../api/discounts';
import { productImageUrl } from '../api/client';
import { useCartStore } from '../store/cartStore';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { PageLoader } from '../components/ui/Loading';

export function ProductDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [qty, setQty] = useState(1);
  const [imgError, setImgError] = useState(false);
  const [added, setAdded] = useState(false);
  const addItem = useCartStore(s => s.addItem);

  const { data: product, isLoading, error } = useQuery({
    queryKey: ['product', id],
    queryFn: () => getProduct(id!),
    enabled: !!id,
  });

  const { data: discount } = useQuery({
    queryKey: ['discount', id],
    queryFn: () => getDiscountByProduct(id!),
    enabled: !!id,
    retry: false,
  });

  if (isLoading) return <PageLoader />;
  if (error || !product) {
    return (
      <div className="flex min-h-[60vh] flex-col items-center justify-center gap-4 text-center">
        <div className="text-6xl">😕</div>
        <h2 className="text-2xl font-bold text-slate-800">Product not found</h2>
        <Button onClick={() => navigate('/products')} variant="secondary">
          <ArrowLeft className="h-4 w-4" /> Back to Products
        </Button>
      </div>
    );
  }

  const isDiscountValid = discount && new Date(discount.expired_at) > new Date();
  const discountedPrice = isDiscountValid
    ? product.price * (1 - discount.percentage / 100)
    : null;

  const savings = discountedPrice !== null ? (product.price - discountedPrice) * qty : 0;

  const handleAddToCart = () => {
    for (let i = 0; i < qty; i++) {
      addItem(product, isDiscountValid ? discount : undefined);
    }
    setAdded(true);
    setTimeout(() => setAdded(false), 2000);
  };

  return (
    <div className="min-h-screen bg-slate-50 py-8">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        {/* Breadcrumb */}
        <nav className="mb-6 flex items-center gap-1 text-sm text-slate-500">
          <Link to="/" className="hover:text-indigo-600">Home</Link>
          <ChevronRight className="h-3.5 w-3.5" />
          <Link to="/products" className="hover:text-indigo-600">Products</Link>
          <ChevronRight className="h-3.5 w-3.5" />
          <Link to={`/products?category=${encodeURIComponent(product.category)}`} className="hover:text-indigo-600">
            {product.category}
          </Link>
          <ChevronRight className="h-3.5 w-3.5" />
          <span className="font-medium text-slate-700">{product.name}</span>
        </nav>

        <div className="grid gap-10 lg:grid-cols-2">
          {/* Image */}
          <div className="overflow-hidden rounded-3xl bg-white shadow-md">
            {!imgError ? (
              <img
                src={productImageUrl(product.product_id)}
                alt={product.name}
                className="h-full min-h-[400px] w-full object-cover"
                onError={() => setImgError(true)}
              />
            ) : (
              <div className="flex min-h-[400px] items-center justify-center bg-gradient-to-br from-slate-100 to-slate-200">
                <div className="text-center">
                  <div className="mx-auto mb-3 flex h-20 w-20 items-center justify-center rounded-3xl bg-indigo-100">
                    <Tag className="h-10 w-10 text-indigo-400" />
                  </div>
                  <p className="text-slate-400">No image available</p>
                </div>
              </div>
            )}
          </div>

          {/* Details */}
          <div className="flex flex-col">
            <div className="mb-3 flex gap-2">
              <Badge variant="indigo">{product.category}</Badge>
              {product.sub_category && <Badge variant="slate">{product.sub_category}</Badge>}
              {isDiscountValid && (
                <Badge variant="red">Sale -{discount.percentage}%</Badge>
              )}
            </div>

            <h1 className="mb-3 text-3xl font-bold text-slate-900">{product.name}</h1>

            <p className="mb-6 text-slate-600 leading-relaxed">{product.description}</p>

            {/* Price */}
            <div className="mb-6 rounded-2xl bg-slate-50 p-5">
              {discountedPrice !== null ? (
                <>
                  <div className="flex items-baseline gap-3">
                    <span className="text-4xl font-extrabold text-indigo-600">
                      ${discountedPrice.toFixed(2)}
                    </span>
                    <span className="text-xl text-slate-400 line-through">${product.price.toFixed(2)}</span>
                  </div>
                  <p className="mt-1 text-sm text-green-600 font-medium">
                    You save ${savings.toFixed(2)} ({isDiscountValid && discount.percentage}% off)
                  </p>
                  <p className="mt-0.5 text-xs text-slate-400">
                    Offer expires: {isDiscountValid && new Date(discount.expired_at).toLocaleDateString()}
                  </p>
                </>
              ) : (
                <span className="text-4xl font-extrabold text-slate-800">${product.price.toFixed(2)}</span>
              )}
            </div>

            {/* Stock */}
            <div className="mb-6 flex items-center gap-2 text-sm">
              <Package className="h-4 w-4 text-slate-400" />
              {product.quantity > 0 ? (
                <span className="text-green-600 font-medium">{product.quantity} in stock</span>
              ) : (
                <span className="text-red-500 font-medium">Out of stock</span>
              )}
            </div>

            {/* Quantity */}
            {product.quantity > 0 && (
              <div className="mb-6">
                <label className="mb-2 block text-sm font-medium text-slate-700">Quantity</label>
                <div className="flex items-center gap-3">
                  <button
                    onClick={() => setQty(q => Math.max(1, q - 1))}
                    className="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200 bg-white text-slate-600 transition hover:bg-slate-50"
                  >
                    <Minus className="h-4 w-4" />
                  </button>
                  <span className="w-12 text-center text-lg font-semibold text-slate-800">{qty}</span>
                  <button
                    onClick={() => setQty(q => Math.min(product.quantity, q + 1))}
                    className="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200 bg-white text-slate-600 transition hover:bg-slate-50"
                  >
                    <Plus className="h-4 w-4" />
                  </button>
                  <span className="text-sm text-slate-400">max {product.quantity}</span>
                </div>
              </div>
            )}

            {/* Actions */}
            <div className="flex gap-3">
              <Button
                onClick={handleAddToCart}
                disabled={product.quantity === 0}
                fullWidth
                size="lg"
                className={added ? 'bg-green-500 hover:bg-green-600' : ''}
              >
                <ShoppingCart className="h-5 w-5" />
                {added ? 'Added to Cart!' : 'Add to Cart'}
              </Button>
              <Link to="/cart" className="shrink-0">
                <Button variant="secondary" size="lg">View Cart</Button>
              </Link>
            </div>

            <Link
              to="/products"
              className="mt-6 flex items-center gap-1 text-sm text-slate-500 transition hover:text-indigo-600"
            >
              <ArrowLeft className="h-4 w-4" /> Back to Products
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
