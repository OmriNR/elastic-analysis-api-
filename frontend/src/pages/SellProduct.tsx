import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { PackagePlus, ChevronDown } from 'lucide-react';
import { useAuthStore } from '../store/authStore';
import { createProduct } from '../api/products';
import { Button } from '../components/ui/Button';

const CATEGORIES = ['Electronics', 'Clothing', 'Books', 'Home', 'Sports', 'Beauty', 'Toys', 'Food'];

const EMPTY_FORM = {
  name: '',
  description: '',
  category: '',
  sub_category: '',
  price: '',
  quantity: '',
};

export function SellProduct() {
  const { user } = useAuthStore();
  const navigate = useNavigate();
  const [form, setForm] = useState(EMPTY_FORM);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!user) navigate('/login');
  }, [user, navigate]);

  if (!user) return null;

  const set = (field: keyof typeof EMPTY_FORM) => (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => setForm(f => ({ ...f, [field]: e.target.value }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    const price = parseFloat(form.price);
    const quantity = parseInt(form.quantity, 10);

    if (!form.name.trim() || !form.description.trim() || !form.category) {
      setError('Name, description, and category are required.');
      return;
    }
    if (isNaN(price) || price <= 0) {
      setError('Price must be a positive number.');
      return;
    }
    if (isNaN(quantity) || quantity < 0) {
      setError('Quantity must be zero or more.');
      return;
    }

    setLoading(true);
    try {
      const created = await createProduct({
        owner_id: user.user_id,
        name: form.name.trim(),
        description: form.description.trim(),
        category: form.category,
        sub_category: form.sub_category.trim() || undefined,
        price,
        quantity,
      });
      navigate(`/products/${created.product_id}`);
    } catch {
      setError('Failed to create product. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const inputClass =
    'w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-800 outline-none transition focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100';

  return (
    <div className="min-h-screen bg-slate-50 py-10">
      <div className="mx-auto max-w-xl px-4">
        <div className="mb-8 flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-indigo-600 text-white">
            <PackagePlus className="h-5 w-5" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-slate-900">List a New Product</h1>
            <p className="text-sm text-slate-500">Fill in the details to add your product to the store.</p>
          </div>
        </div>

        <form onSubmit={handleSubmit} className="rounded-2xl bg-white p-6 shadow-md space-y-5">
          {error && (
            <div className="rounded-xl bg-red-50 border border-red-200 px-4 py-3 text-sm text-red-600">
              {error}
            </div>
          )}

          <div>
            <label className="mb-1.5 block text-xs font-medium text-slate-600">Product Name *</label>
            <input
              type="text"
              placeholder="e.g. Wireless Headphones"
              value={form.name}
              onChange={set('name')}
              className={inputClass}
              maxLength={120}
            />
          </div>

          <div>
            <label className="mb-1.5 block text-xs font-medium text-slate-600">Description *</label>
            <textarea
              placeholder="Describe your product..."
              value={form.description}
              onChange={set('description')}
              rows={4}
              className={`${inputClass} resize-none`}
              maxLength={1000}
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="mb-1.5 block text-xs font-medium text-slate-600">Category *</label>
              <div className="relative">
                <select
                  value={form.category}
                  onChange={set('category')}
                  className={`${inputClass} appearance-none pr-8`}
                >
                  <option value="">Select…</option>
                  {CATEGORIES.map(c => (
                    <option key={c} value={c}>{c}</option>
                  ))}
                </select>
                <ChevronDown className="pointer-events-none absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
              </div>
            </div>

            <div>
              <label className="mb-1.5 block text-xs font-medium text-slate-600">Sub-category</label>
              <input
                type="text"
                placeholder="e.g. Audio"
                value={form.sub_category}
                onChange={set('sub_category')}
                className={inputClass}
                maxLength={60}
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="mb-1.5 block text-xs font-medium text-slate-600">Price ($) *</label>
              <input
                type="number"
                placeholder="0.00"
                value={form.price}
                onChange={set('price')}
                className={inputClass}
                min="0"
                step="0.01"
              />
            </div>

            <div>
              <label className="mb-1.5 block text-xs font-medium text-slate-600">Quantity *</label>
              <input
                type="number"
                placeholder="0"
                value={form.quantity}
                onChange={set('quantity')}
                className={inputClass}
                min="0"
                step="1"
              />
            </div>
          </div>

          <Button type="submit" loading={loading} className="w-full justify-center">
            List Product
          </Button>
        </form>
      </div>
    </div>
  );
}
