import { apiClient } from './client';
import type { User, UserProperties } from '../types';

export const loginUser = (email: string, password: string) =>
  apiClient.post<User>('/api/users/GetUser', { email, password }).then(r => r.data);

export const createUser = (user: Omit<User, 'user_id' | 'created_at'>) =>
  apiClient.post<User>('/api/users/create', user).then(r => r.data);

export const updateUser = (user: User) =>
  apiClient.put<User>('/api/users/update', user).then(r => r.data);

export const getUserProperties = (userId: string) =>
  apiClient.get<UserProperties>(`/api/usersproperties/${userId}`).then(r => r.data);

export const createUserProperties = (props: Omit<UserProperties, 'created_at'>) =>
  apiClient.post<UserProperties>('/api/usersproperties', props).then(r => r.data);

export const updateUserProperties = (props: UserProperties) =>
  apiClient.put<UserProperties>('/api/usersproperties', props).then(r => r.data);

export const deleteUserProperties = (userId: string) =>
  apiClient.delete(`/api/usersproperties/${userId}`).then(r => r.data);
