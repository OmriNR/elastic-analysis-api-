import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { CartItem, Product, Discount } from '../types';

interface CartState {
  items: CartItem[];
  addItem: (product: Product, discount?: Discount) => void;
  removeItem: (productId: string) => void;
  updateQty: (productId: string, quantity: number) => void;
  clearCart: () => void;
  totalItems: () => number;
  subtotal: () => number;
  totalDiscount: () => number;
  total: () => number;
}

export const useCartStore = create<CartState>()(
  persist(
    (set, get) => ({
      items: [],

      addItem: (product, discount) =>
        set((state) => {
          const existing = state.items.find(i => i.product.productId === product.productId);
          if (existing) {
            return {
              items: state.items.map(i =>
                i.product.productId === product.productId
                  ? { ...i, quantity: Math.min(i.quantity + 1, product.quantity) }
                  : i
              ),
            };
          }
          return { items: [...state.items, { product, quantity: 1, discount }] };
        }),

      removeItem: (productId) =>
        set((state) => ({ items: state.items.filter(i => i.product.productId !== productId) })),

      updateQty: (productId, quantity) =>
        set((state) => ({
          items: quantity <= 0
            ? state.items.filter(i => i.product.productId !== productId)
            : state.items.map(i =>
                i.product.productId === productId ? { ...i, quantity } : i
              ),
        })),

      clearCart: () => set({ items: [] }),

      totalItems: () => get().items.reduce((sum, i) => sum + i.quantity, 0),

      subtotal: () =>
        get().items.reduce((sum, i) => sum + i.product.price * i.quantity, 0),

      totalDiscount: () =>
        get().items.reduce((sum, i) => {
          if (!i.discount) return sum;
          const expires = new Date(i.discount.expiredAt);
          if (expires < new Date()) return sum;
          return sum + i.product.price * i.quantity * (i.discount.percentage / 100);
        }, 0),

      total: () => get().subtotal() - get().totalDiscount(),
    }),
    { name: 'cart-storage' }
  )
);
