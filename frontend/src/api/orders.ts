import { apiClient } from './client';
import type { Order } from '../types';

export const getOrder = (orderId: string) =>
  apiClient.get<Order>(`/api/orders/${orderId}`).then(r => r.data);

export const getOrdersByCustomer = (customerId: string) =>
  apiClient.get<Order[]>(`/api/orders/customer/${customerId}`).then(r => r.data);

export const getOrdersByProduct = (productId: string) =>
  apiClient.get<Order[]>(`/api/orders/product/${productId}`).then(r => r.data);

export const createOrder = (order: Omit<Order, 'order_id' | 'timestamp'>) =>
  apiClient.post<Order>('/api/orders', order).then(r => r.data);
