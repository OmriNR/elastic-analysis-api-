import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { User, Calendar, Package, X, LogIn, UserPlus } from 'lucide-react';
import { getUserById } from '../api/users';
import { getProductsByUser } from '../api/products';
import { useAuthStore } from '../store/authStore';
import { useCartStore } from '../store/cartStore';
import { ProductCard } from '../components/products/ProductCard';
import { PageLoader, SkeletonCard } from '../components/ui/Loading';
import type { Product } from '../types';

function AuthModal({ onClose }: { onClose: () => void }) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
      <div className="relative mx-4 w-full max-w-sm rounded-2xl bg-white p-6 shadow-2xl">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 rounded-lg p-1 text-slate-400 hover:bg-slate-100 hover:text-slate-600"
        >
          <X className="h-5 w-5" />
        </button>

        <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-2xl bg-indigo-100">
          <LogIn className="h-6 w-6 text-indigo-600" />
        </div>
        <h2 className="mb-1 text-lg font-bold text-slate-900">Sign in to order</h2>
        <p className="mb-6 text-sm text-slate-500">
          You need an account to place an order. Sign in or create a free account to continue.
        </p>

        <div className="flex flex-col gap-3">
          <Link
            to="/login"
            className="flex items-center justify-center gap-2 rounded-xl bg-indigo-600 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-indigo-700"
          >
            <LogIn className="h-4 w-4" /> Sign In
          </Link>
          <Link
            to="/register"
            className="flex items-center justify-center gap-2 rounded-xl border border-indigo-200 bg-indigo-50 px-4 py-2.5 text-sm font-medium text-indigo-600 transition hover:bg-indigo-100"
          >
            <UserPlus className="h-4 w-4" /> Create Account
          </Link>
        </div>
      </div>
    </div>
  );
}

export function SellerProfile() {
  const { userId } = useParams<{ userId: string }>();
  const { user: currentUser } = useAuthStore();
  const addItem = useCartStore(s => s.addItem);
  const [showAuthModal, setShowAuthModal] = useState(false);

  const { data: seller, isLoading: sellerLoading, isError: sellerError } = useQuery({
    queryKey: ['user', userId],
    queryFn: () => getUserById(userId!),
    enabled: !!userId,
  });

  const { data: products = [], isLoading: productsLoading } = useQuery({
    queryKey: ['products', 'user', userId],
    queryFn: () => getProductsByUser(userId!),
    enabled: !!userId,
  });

  const handleCartClick = (product: Product) => (_e: React.MouseEvent) => {
    if (!currentUser) {
      setShowAuthModal(true);
    } else {
      addItem(product);
    }
  };

  if (sellerLoading) return <PageLoader />;

  if (sellerError || !seller) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <p className="text-2xl font-bold text-slate-700">Seller not found</p>
          <Link to="/products" className="mt-4 inline-block text-sm text-indigo-600 hover:underline">
            Back to Products
          </Link>
        </div>
      </div>
    );
  }

  const joinedDate = new Date(seller.createdAt).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
  });

  return (
    <div className="min-h-screen bg-slate-50">
      {showAuthModal && <AuthModal onClose={() => setShowAuthModal(false)} />}

      {/* Seller header */}
      <div className="border-b border-slate-200 bg-white">
        <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
          <div className="flex items-center gap-5">
            <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-indigo-100">
              <User className="h-8 w-8 text-indigo-600" />
            </div>
            <div>
              <h1 className="text-2xl font-bold text-slate-900">{seller.userName}</h1>
              <div className="mt-1 flex items-center gap-4 text-sm text-slate-500">
                <span className="flex items-center gap-1.5">
                  <Calendar className="h-4 w-4" /> Member since {joinedDate}
                </span>
                <span className="flex items-center gap-1.5">
                  <Package className="h-4 w-4" /> {products.length} listing{products.length !== 1 ? 's' : ''}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Products grid */}
      <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
        <h2 className="mb-6 text-lg font-semibold text-slate-800">Products by {seller.userName}</h2>

        {productsLoading ? (
          <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {Array.from({ length: 4 }).map((_, i) => <SkeletonCard key={i} />)}
          </div>
        ) : products.length === 0 ? (
          <div className="flex flex-col items-center justify-center rounded-2xl bg-white py-20 text-center shadow-sm">
            <Package className="mb-3 h-12 w-12 text-slate-300" />
            <h3 className="text-lg font-semibold text-slate-700">No products yet</h3>
            <p className="text-sm text-slate-400">This seller hasn't listed any products.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {products.map(product => (
              <ProductCard
                key={product.product_id}
                product={product}
                onCartClick={handleCartClick(product)}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
