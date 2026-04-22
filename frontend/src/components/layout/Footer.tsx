import { Link } from 'react-router-dom';
import { Store, Github } from 'lucide-react';

export function Footer() {
  return (
    <footer className="border-t border-slate-200 bg-white">
      <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="flex flex-col items-center justify-between gap-4 md:flex-row">
          <Link to="/" className="flex items-center gap-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-gradient-to-br from-indigo-600 to-purple-600">
              <Store className="h-4 w-4 text-white" />
            </div>
            <span className="font-bold text-slate-800">E-Shop</span>
          </Link>
          <div className="flex gap-6 text-sm text-slate-500">
            <Link to="/" className="transition-colors hover:text-indigo-600">Home</Link>
            <Link to="/products" className="transition-colors hover:text-indigo-600">Products</Link>
            <Link to="/cart" className="transition-colors hover:text-indigo-600">Cart</Link>
          </div>
          <div className="flex items-center gap-2 text-sm text-slate-400">
            <Github className="h-4 w-4" />
            <span>© {new Date().getFullYear()} E-Shop</span>
          </div>
        </div>
      </div>
    </footer>
  );
}
