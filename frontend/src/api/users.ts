import { apiClient } from './client';
import type { User, PublicUser } from '../types';

export interface LoginResponse {
  user: User;
  token: string;
}

export const loginUser = (email: string, password: string) =>
  apiClient.post<LoginResponse>('/api/users/GetUser', { email, password }).then(r => r.data);

export const createUser = (user: Omit<User, 'user_id' | 'created_at'>) =>
  apiClient.post<LoginResponse>('/api/users/create', user).then(r => r.data);

export const updateUser = (user: User) =>
  apiClient.put<User>('/api/users/update', user).then(r => r.data);

export const getAllUsers = (requesterId: string) =>
  apiClient.get<User[]>(`/api/users/getAllUsers(${requesterId}`).then(r => r.data);

export const setAdmin = (requesterId: string, targetUserId: string) =>
  apiClient.post<string>('/api/users/setAdmin', { requested_user_id: requesterId, target_user_id: targetUserId }).then(r => r.data);

export const setActivity = (requesterId: string, targetUserId: string) =>
  apiClient.post<string>('/api/users/setActivity', { requested_user_id: requesterId, target_user_id: targetUserId }).then(r => r.data);

export const getUserById = (id: string) =>
  apiClient.get<PublicUser>(`/api/users/${id}`).then(r => r.data);
