import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { User, UserProperties } from '../types';

interface AuthState {
  user: User | null;
  userProps: UserProperties | null;
  setUser: (user: User | null) => void;
  setUserProps: (props: UserProperties | null) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      userProps: null,
      setUser: (user) => set({ user }),
      setUserProps: (userProps) => set({ userProps }),
      logout: () => set({ user: null, userProps: null }),
    }),
    { name: 'auth-storage' }
  )
);
