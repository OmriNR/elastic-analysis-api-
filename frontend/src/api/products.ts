import { apiClient } from './client';
import type { Product } from '../types';

export const getProduct = (id: string) =>
  apiClient.get<Product>(`/api/products/${id}`).then(r => r.data);

export const getAllProducts = () =>
  apiClient.get<Product[]>('/api/products/all').then(r => r.data);

export const getAllCategories = () =>
  apiClient.get<string[]>('/api/products/categories/all').then(r => r.data);

export const getProductsByCategory = (category: string) =>
  apiClient.get<Product[]>(`/api/products/categories/${category}`).then(r => r.data);

export const getProductsByUser = (userId: string) =>
  apiClient.get<Product[]>(`/api/products/users/${userId}`).then(r => r.data);

export const getProductsByIds = (ids: string[]) =>
  apiClient.post<Product[]>('/api/products/multi', ids).then(r => r.data);

export const createProduct = (product: Omit<Product, 'product_id'>) =>
  apiClient.post<Product>('/api/products', product).then(r => r.data);

export const updateProduct = (product: Product) =>
  apiClient.put<Product>('/api/products', product).then(r => r.data);

export const deleteProduct = (id: string) =>
  apiClient.delete(`/api/products/${id}`).then(r => r.data);

export const uploadProductImage = async (productId: string, file: File): Promise<void> => {
  const formData = new FormData();
  formData.append('file', file);
  await apiClient.post(`/Images/UploadProductImage/${productId}`, formData, {
    headers: { 'Content-Type': undefined },
  });
};
