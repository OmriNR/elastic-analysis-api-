import { apiClient } from './client';
import type { Discount } from '../types';

export const getDiscount = (id: string) =>
  apiClient.get<Discount>(`/api/discounts/${id}`).then(r => r.data);

export const getDiscountByProduct = (productId: string) =>
  apiClient.get<Discount>(`/api/discounts/productId/${productId}`).then(r => r.data);

export const getAllActiveDiscounts = () =>
  apiClient.get<Discount[]>('/api/discounts/all').then(r => r.data);

export const createDiscount = (discount: Omit<Discount, 'discount_id'>) =>
  apiClient.post<Discount>('/api/discounts', discount).then(r => r.data);

export const createDiscountForUser = (userId: string, discount: Omit<Discount, 'discount_id'>) =>
  apiClient.post<Discount[]>(`/api/discounts/user/${userId}`, discount).then(r => r.data);

export const createDiscountForCategory = (category: string, discount: Omit<Discount, 'discount_id'>) =>
  apiClient.post<Discount[]>(`/api/discounts/category/${category}`, discount).then(r => r.data);

export const deleteDiscount = (id: string) =>
  apiClient.delete(`/api/discounts/${id}`).then(r => r.data);
