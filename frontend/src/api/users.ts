import { apiClient } from './client';
import type { User } from '../types';

export const loginUser = (email: string, password: string) =>
  apiClient.post<User>('/api/users/GetUser', { email, password }).then(r => r.data);

export const createUser = (user: Omit<User, 'user_id' | 'created_at'>) =>
  apiClient.post<User>('/api/users/create', user).then(r => r.data);

export const updateUser = (user: User) =>
  apiClient.put<User>('/api/users/update', user).then(r => r.data);
