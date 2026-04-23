import { useState, useMemo } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useSearchParams } from 'react-router-dom';
import { SlidersHorizontal, X, Search } from 'lucide-react';
import { getAllProducts, getAllCategories } from '../api/products';
import { ProductCard } from '../components/products/ProductCard';
import { PageLoader, SkeletonCard } from '../components/ui/Loading';

type SortKey = 'name-asc' | 'name-desc' | 'price-asc' | 'price-desc';

const sortOptions: { value: SortKey; label: string }[] = [
  { value: 'name-asc', label: 'Name (A → Z)' },
  { value: 'name-desc', label: 'Name (Z → A)' },
  { value: 'price-asc', label: 'Price (Low → High)' },
  { value: 'price-desc', label: 'Price (High → Low)' },
];

export function Products() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [search, setSearch] = useState('');
  const [sort, setSort] = useState<SortKey>('name-asc');
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const selectedCategory = searchParams.get('category') ?? '';

  const { data: products = [], isLoading: productsLoading } = useQuery({
    queryKey: ['products'],
    queryFn: getAllProducts,
  });

  const { data: categories = [], isLoading: catsLoading } = useQuery({
    queryKey: ['categories'],
    queryFn: getAllCategories,
  });

  const filtered = useMemo(() => {
    let list = [...products];
    if (selectedCategory) list = list.filter(p => p.category === selectedCategory);
    if (search.trim()) {
      const q = search.toLowerCase();
      list = list.filter(p =>
        p.name.toLowerCase().includes(q) || p.description.toLowerCase().includes(q)
      );
    }
    list.sort((a, b) => {
      if (sort === 'name-asc') return a.name.localeCompare(b.name);
      if (sort === 'name-desc') return b.name.localeCompare(a.name);
      if (sort === 'price-asc') return a.price - b.price;
      return b.price - a.price;
    });
    return list;
  }, [products, selectedCategory, search, sort]);

  const setCategory = (cat: string) => {
    if (cat) setSearchParams({ category: cat });
    else setSearchParams({});
    setSidebarOpen(false);
  };

  if (productsLoading && catsLoading) return <PageLoader />;

  return (
    <div className="min-h-screen bg-slate-50">
      <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-6">
          <h1 className="text-3xl font-bold text-slate-900">Products</h1>
          <p className="text-sm text-slate-500">{filtered.length} items found</p>
        </div>

        {/* Search + Sort bar */}
        <div className="mb-6 flex gap-3">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              placeholder="Search products..."
              value={search}
              onChange={e => setSearch(e.target.value)}
              className="w-full rounded-xl border border-slate-200 bg-white py-2.5 pl-10 pr-4 text-sm text-slate-800 outline-none transition focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
            />
          </div>
          <select
            value={sort}
            onChange={e => setSort(e.target.value as SortKey)}
            className="rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-700 outline-none focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
          >
            {sortOptions.map(o => (
              <option key={o.value} value={o.value}>{o.label}</option>
            ))}
          </select>
          <button
            onClick={() => setSidebarOpen(o => !o)}
            className="flex items-center gap-2 rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm font-medium text-slate-700 transition hover:bg-slate-50 lg:hidden"
          >
            <SlidersHorizontal className="h-4 w-4" /> Filters
          </button>
        </div>

        {/* Active category filter pill */}
        {selectedCategory && (
          <div className="mb-4 flex items-center gap-2">
            <span className="text-sm text-slate-600">Filtered by:</span>
            <button
              onClick={() => setCategory('')}
              className="flex items-center gap-1.5 rounded-full bg-indigo-100 px-3 py-1 text-sm font-medium text-indigo-700 transition hover:bg-indigo-200"
            >
              {selectedCategory} <X className="h-3.5 w-3.5" />
            </button>
          </div>
        )}

        <div className="flex gap-6">
          {/* Sidebar */}
          <aside className={`
            ${sidebarOpen ? 'flex' : 'hidden'} lg:flex
            w-56 shrink-0 flex-col gap-2
          `}>
            <div className="sticky top-24 rounded-2xl bg-white p-5 shadow-sm">
              <h3 className="mb-3 font-semibold text-slate-800">Categories</h3>
              <button
                onClick={() => setCategory('')}
                className={`w-full rounded-lg px-3 py-2 text-left text-sm transition ${
                  !selectedCategory
                    ? 'bg-indigo-50 font-medium text-indigo-600'
                    : 'text-slate-600 hover:bg-slate-50'
                }`}
              >
                All Products
              </button>
              {categories.map(cat => (
                <button
                  key={cat}
                  onClick={() => setCategory(cat)}
                  className={`w-full rounded-lg px-3 py-2 text-left text-sm transition ${
                    selectedCategory === cat
                      ? 'bg-indigo-50 font-medium text-indigo-600'
                      : 'text-slate-600 hover:bg-slate-50'
                  }`}
                >
                  {cat}
                </button>
              ))}
            </div>
          </aside>

          {/* Grid */}
          <div className="flex-1">
            {productsLoading ? (
              <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 xl:grid-cols-3">
                {Array.from({ length: 6 }).map((_, i) => <SkeletonCard key={i} />)}
              </div>
            ) : filtered.length === 0 ? (
              <div className="flex flex-col items-center justify-center rounded-2xl bg-white py-20 text-center shadow-sm">
                <div className="mb-3 text-5xl">🔍</div>
                <h3 className="text-lg font-semibold text-slate-700">No products found</h3>
                <p className="text-sm text-slate-400">Try adjusting your search or filter.</p>
              </div>
            ) : (
              <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 xl:grid-cols-3">
                {filtered.map(product => (
                  <ProductCard key={product.product_id} product={product} />
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
