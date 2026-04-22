import { apiClient } from './client';
import type { Discount } from '../types';

export const getDiscount = (id: string) =>
  apiClient.get<Discount>(`/api/discounts/${id}`).then(r => r.data);

export const getDiscountByProduct = (productId: string) =>
  apiClient.get<Discount>(`/api/discounts/productId/${productId}`).then(r => r.data);

export const createDiscount = (discount: Omit<Discount, 'discountId'>) =>
  apiClient.post<Discount>('/api/discounts', discount).then(r => r.data);
