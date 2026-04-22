import axios from 'axios';

export const apiClient = axios.create({
  baseURL: '',
  headers: { 'Content-Type': 'application/json' },
});

export const productImageUrl = (productId: string) =>
  `/Images/GetProductImage/${productId}`;

export const userImageUrl = (userId: string) =>
  `/Images/GetUserImage/${userId}`;
