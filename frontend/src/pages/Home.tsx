import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { ArrowRight, ShoppingBag, Zap, Shield, Truck } from 'lucide-react';
import { getAllProducts, getAllCategories } from '../api/products';
import { ProductCard } from '../components/products/ProductCard';
import { Button } from '../components/ui/Button';
import { SkeletonCard } from '../components/ui/Loading';

const categoryIcons: Record<string, string> = {
  Electronics: '💻',
  Clothing: '👕',
  Books: '📚',
  Home: '🏠',
  Sports: '⚽',
  Beauty: '✨',
  Toys: '🎮',
  Food: '🍎',
};

const features = [
  { icon: Zap, title: 'Fast Delivery', desc: 'Lightning-fast shipping to your door.' },
  { icon: Shield, title: 'Secure Payments', desc: 'Your transactions are always protected.' },
  { icon: Truck, title: 'Free Returns', desc: 'Hassle-free returns within 30 days.' },
];

export function Home() {
  const { data: products = [], isLoading: productsLoading } = useQuery({
    queryKey: ['products'],
    queryFn: getAllProducts,
  });

  const { data: categories = [] } = useQuery({
    queryKey: ['categories'],
    queryFn: getAllCategories,
  });

  const featured = products.slice(0, 8);

  return (
    <div>
      {/* Hero */}
      <section className="relative overflow-hidden bg-gradient-to-br from-indigo-950 via-purple-900 to-indigo-800 py-24 text-white">
        <div className="absolute inset-0 overflow-hidden">
          <div className="absolute -left-40 -top-40 h-80 w-80 rounded-full bg-purple-500/20 blur-3xl" />
          <div className="absolute -bottom-40 -right-20 h-96 w-96 rounded-full bg-indigo-400/20 blur-3xl" />
        </div>
        <div className="relative mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="mx-auto max-w-2xl text-center">
            <div className="mb-6 inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-4 py-2 text-sm backdrop-blur-sm">
              <ShoppingBag className="h-4 w-4" />
              <span>New arrivals every week</span>
            </div>
            <h1 className="mb-6 text-5xl font-extrabold leading-tight tracking-tight sm:text-6xl">
              Discover Products
              <br />
              <span className="bg-gradient-to-r from-purple-300 to-pink-300 bg-clip-text text-transparent">
                You'll Love
              </span>
            </h1>
            <p className="mb-10 text-lg text-indigo-200">
              Shop the latest trends across hundreds of categories. Fast shipping, easy returns, and exclusive deals.
            </p>
            <div className="flex flex-col gap-3 sm:flex-row sm:justify-center">
              <Link to="/products">
                <Button size="lg" className="bg-white text-indigo-700 hover:bg-indigo-50">
                  Shop Now <ArrowRight className="h-5 w-5" />
                </Button>
              </Link>
            </div>
          </div>
        </div>
      </section>

      {/* Features */}
      <section className="border-b border-slate-100 bg-white py-10">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-3">
            {features.map(({ icon: Icon, title, desc }) => (
              <div key={title} className="flex items-center gap-4 rounded-2xl border border-slate-100 p-5">
                <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-indigo-100">
                  <Icon className="h-6 w-6 text-indigo-600" />
                </div>
                <div>
                  <p className="font-semibold text-slate-800">{title}</p>
                  <p className="text-sm text-slate-500">{desc}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Categories */}
      {categories.length > 0 && (
        <section className="bg-slate-50 py-16">
          <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
            <div className="mb-8 flex items-center justify-between">
              <div>
                <h2 className="text-2xl font-bold text-slate-900">Shop by Category</h2>
                <p className="text-sm text-slate-500">Find exactly what you're looking for</p>
              </div>
              <Link to="/products" className="flex items-center gap-1 text-sm font-medium text-indigo-600 hover:text-indigo-700">
                All categories <ArrowRight className="h-4 w-4" />
              </Link>
            </div>
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6">
              {categories.slice(0, 12).map(cat => (
                <Link
                  key={cat}
                  to={`/products?category=${encodeURIComponent(cat)}`}
                  className="group flex flex-col items-center justify-center gap-2 rounded-2xl bg-white p-5 text-center shadow-sm transition-all duration-200 hover:-translate-y-1 hover:shadow-md"
                >
                  <span className="text-3xl">{categoryIcons[cat] ?? '🛍️'}</span>
                  <span className="text-sm font-medium text-slate-700 group-hover:text-indigo-600 transition-colors">
                    {cat}
                  </span>
                </Link>
              ))}
            </div>
          </div>
        </section>
      )}

      {/* Featured Products */}
      <section className="py-16">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="mb-8 flex items-center justify-between">
            <div>
              <h2 className="text-2xl font-bold text-slate-900">Featured Products</h2>
              <p className="text-sm text-slate-500">Hand-picked products just for you</p>
            </div>
            <Link to="/products" className="flex items-center gap-1 text-sm font-medium text-indigo-600 hover:text-indigo-700">
              View all <ArrowRight className="h-4 w-4" />
            </Link>
          </div>
          <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {productsLoading
              ? Array.from({ length: 8 }).map((_, i) => <SkeletonCard key={i} />)
              : featured.map(product => (
                  <ProductCard key={product.product_id} product={product} />
                ))}
          </div>
          {!productsLoading && products.length > 8 && (
            <div className="mt-10 text-center">
              <Link to="/products">
                <Button variant="secondary" size="lg">
                  View All {products.length} Products <ArrowRight className="h-5 w-5" />
                </Button>
              </Link>
            </div>
          )}
        </div>
      </section>

      {/* CTA Banner */}
      <section className="bg-gradient-to-r from-indigo-600 to-purple-600 py-16 text-white">
        <div className="mx-auto max-w-7xl px-4 text-center sm:px-6 lg:px-8">
          <h2 className="mb-3 text-3xl font-bold">Ready to start shopping?</h2>
          <p className="mb-8 text-indigo-200">Join thousands of happy customers and discover amazing deals.</p>
          <Link to="/register">
            <Button size="lg" className="bg-white text-indigo-700 hover:bg-indigo-50">
              Create an Account <ArrowRight className="h-5 w-5" />
            </Button>
          </Link>
        </div>
      </section>
    </div>
  );
}
