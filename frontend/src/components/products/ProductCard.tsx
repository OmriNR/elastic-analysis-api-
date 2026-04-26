import { useState } from 'react';
import { Link } from 'react-router-dom';
import { ShoppingCart, Tag, User } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import type { Product, Discount } from '../../types';
import { Badge } from '../ui/Badge';
import { productImageUrl } from '../../api/client';
import { useCartStore } from '../../store/cartStore';
import { getUserById } from '../../api/users';

interface ProductCardProps {
  product: Product;
  discount?: Discount;
  onCartClick?: (e: React.MouseEvent) => void;
}

export function ProductCard({ product, discount, onCartClick }: ProductCardProps) {
  const addItem = useCartStore(s => s.addItem);
  const [imgError, setImgError] = useState(false);

  const { data: owner } = useQuery({
    queryKey: ['user', product.owner_id],
    queryFn: () => getUserById(product.owner_id),
    staleTime: 5 * 60 * 1000,
  });

  const isDiscountValid = discount && new Date(discount.expired_at) > new Date();
  const discountedPrice = isDiscountValid
    ? product.price * (1 - discount.percentage / 100)
    : null;

  const handleAddToCart = (e: React.MouseEvent) => {
    e.preventDefault();
    if (onCartClick) {
      onCartClick(e);
    } else {
      addItem(product, isDiscountValid ? discount : undefined);
    }
  };

  return (
    <Link to={`/products/${product.product_id}`} className="group block">
      <div className="relative overflow-hidden rounded-2xl bg-white shadow-md transition-all duration-300 hover:-translate-y-1 hover:shadow-xl">
        <div className="relative h-52 overflow-hidden bg-gradient-to-br from-slate-100 to-slate-200">
          {!imgError ? (
            <img
              src={productImageUrl(product.product_id)}
              alt={product.name}
              className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
              onError={() => setImgError(true)}
            />
          ) : (
            <div className="flex h-full items-center justify-center">
              <div className="text-center">
                <div className="mx-auto mb-2 flex h-14 w-14 items-center justify-center rounded-2xl bg-indigo-100">
                  <Tag className="h-7 w-7 text-indigo-500" />
                </div>
                <p className="text-xs text-slate-400">{product.category}</p>
              </div>
            </div>
          )}
          {isDiscountValid && (
            <div className="absolute left-3 top-3">
              <Badge variant="red">-{discount.percentage}%</Badge>
            </div>
          )}
          {product.quantity === 0 && (
            <div className="absolute inset-0 flex items-center justify-center bg-black/40">
              <span className="rounded-full bg-white px-4 py-1 text-sm font-semibold text-slate-800">Out of Stock</span>
            </div>
          )}
        </div>

        <div className="p-4">
          <Badge variant="indigo" className="mb-2">{product.category}</Badge>
          {product.sub_category && (
            <Badge variant="slate" className="mb-2 ml-1">{product.sub_category}</Badge>
          )}
          <h3 className="mb-1 line-clamp-2 font-semibold text-slate-800 group-hover:text-indigo-600 transition-colors">
            {product.name}
          </h3>
          <p className="mb-3 line-clamp-2 text-xs text-slate-500">{product.description}</p>

          <div className="flex items-center justify-between">
            <div>
              {discountedPrice !== null ? (
                <div className="flex items-baseline gap-1.5">
                  <span className="text-lg font-bold text-indigo-600">${discountedPrice.toFixed(2)}</span>
                  <span className="text-xs text-slate-400 line-through">${product.price.toFixed(2)}</span>
                </div>
              ) : (
                <span className="text-lg font-bold text-slate-800">${product.price.toFixed(2)}</span>
              )}
            </div>
            <button
              onClick={handleAddToCart}
              disabled={product.quantity === 0}
              className="flex h-9 w-9 items-center justify-center rounded-xl bg-indigo-600 text-white shadow-md transition-all hover:bg-indigo-700 hover:shadow-indigo-200/60 hover:shadow-lg disabled:cursor-not-allowed disabled:bg-slate-200 disabled:text-slate-400"
              title="Add to cart"
            >
              <ShoppingCart className="h-4 w-4" />
            </button>
          </div>

          <div className="mt-2 flex items-center justify-between">
            <span className="text-xs text-slate-400">
              {product.quantity > 0 ? `${product.quantity} in stock` : 'Unavailable'}
            </span>
            <Link
              to={`/seller/${product.owner_id}`}
              onClick={e => e.stopPropagation()}
              className="flex items-center gap-1 text-xs text-indigo-500 hover:text-indigo-700 hover:underline"
            >
              <User className="h-3 w-3" />
              {owner?.userName ?? '…'}
            </Link>
          </div>
        </div>
      </div>
    </Link>
  );
}
