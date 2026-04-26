import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Shield, Users, ToggleLeft, ToggleRight, ShieldCheck } from 'lucide-react';
import { getAllUsers, setAdmin, setActivity } from '../api/users';
import { useAuthStore } from '../store/authStore';
import type { User } from '../types';

export function Admin() {
  const { user: currentUser } = useAuthStore();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [actionError, setActionError] = useState<string | null>(null);

  if (!currentUser) {
    navigate('/login');
    return null;
  }

  if (!currentUser.is_admin) {
    navigate('/');
    return null;
  }

  const { data: users = [], isLoading, isError } = useQuery({
    queryKey: ['admin', 'users'],
    queryFn: () => getAllUsers(currentUser.user_id),
  });

  const adminMutation = useMutation({
    mutationFn: (targetId: string) => setAdmin(currentUser.user_id, targetId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin', 'users'] });
      setActionError(null);
    },
    onError: () => setActionError('Failed to update admin status.'),
  });

  const activityMutation = useMutation({
    mutationFn: (targetId: string) => setActivity(currentUser.user_id, targetId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin', 'users'] });
      setActionError(null);
    },
    onError: () => setActionError('Failed to update activity status.'),
  });

  const isPending = (userId: string) =>
    (adminMutation.isPending && adminMutation.variables === userId) ||
    (activityMutation.isPending && activityMutation.variables === userId);

  return (
    <div className="min-h-screen bg-slate-50 py-8">
      <div className="mx-auto max-w-5xl px-4 sm:px-6 lg:px-8">
        <div className="mb-6 flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-purple-100">
            <Shield className="h-5 w-5 text-purple-600" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-slate-900">Admin Panel</h1>
            <p className="text-sm text-slate-500">Manage users, permissions and activity</p>
          </div>
        </div>

        {actionError && (
          <div className="mb-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
            {actionError}
          </div>
        )}

        <div className="rounded-2xl bg-white shadow-sm">
          <div className="flex items-center gap-2 border-b border-slate-100 px-6 py-4">
            <Users className="h-4 w-4 text-slate-500" />
            <h2 className="font-semibold text-slate-800">Registered Users</h2>
            {!isLoading && (
              <span className="ml-auto rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-600">
                {users.length}
              </span>
            )}
          </div>

          {isLoading && (
            <div className="flex items-center justify-center py-16 text-slate-400">
              Loading users…
            </div>
          )}

          {isError && (
            <div className="flex items-center justify-center py-16 text-red-500">
              Failed to load users.
            </div>
          )}

          {!isLoading && !isError && (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-slate-100 text-left text-xs font-semibold uppercase tracking-wide text-slate-400">
                    <th className="px-6 py-3">User</th>
                    <th className="px-6 py-3">Email</th>
                    <th className="px-6 py-3">Status</th>
                    <th className="px-6 py-3">Role</th>
                    <th className="px-6 py-3">Joined</th>
                    <th className="px-6 py-3 text-right">Actions</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50">
                  {users.map((u: User) => (
                    <tr key={u.user_id} className="hover:bg-slate-50/60 transition-colors">
                      <td className="px-6 py-4 font-medium text-slate-800">
                        {u.properties?.user_name || '—'}
                        {u.user_id === currentUser.user_id && (
                          <span className="ml-2 rounded-full bg-indigo-100 px-2 py-0.5 text-[10px] font-semibold text-indigo-600">You</span>
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-500">{u.email}</td>
                      <td className="px-6 py-4">
                        <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${
                          u.is_active ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
                        }`}>
                          {u.is_active ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td className="px-6 py-4">
                        {u.is_admin ? (
                          <span className="rounded-full bg-purple-100 px-2.5 py-0.5 text-xs font-medium text-purple-700">Admin</span>
                        ) : (
                          <span className="rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-500">User</span>
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-400">
                        {new Date(u.created_at).toLocaleDateString()}
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex items-center justify-end gap-2">
                          {!u.is_admin && u.user_id !== currentUser.user_id && (
                            <button
                              onClick={() => adminMutation.mutate(u.user_id)}
                              disabled={isPending(u.user_id)}
                              title="Grant admin"
                              className="flex items-center gap-1.5 rounded-lg border border-purple-200 bg-purple-50 px-3 py-1.5 text-xs font-medium text-purple-700 transition-colors hover:bg-purple-100 disabled:opacity-50"
                            >
                              <ShieldCheck className="h-3.5 w-3.5" />
                              Make Admin
                            </button>
                          )}
                          {u.user_id !== currentUser.user_id && (
                            <button
                              onClick={() => activityMutation.mutate(u.user_id)}
                              disabled={isPending(u.user_id)}
                              title={u.is_active ? 'Deactivate' : 'Activate'}
                              className={`flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-medium transition-colors disabled:opacity-50 ${
                                u.is_active
                                  ? 'border-red-200 bg-red-50 text-red-700 hover:bg-red-100'
                                  : 'border-green-200 bg-green-50 text-green-700 hover:bg-green-100'
                              }`}
                            >
                              {u.is_active
                                ? <><ToggleRight className="h-3.5 w-3.5" /> Deactivate</>
                                : <><ToggleLeft className="h-3.5 w-3.5" /> Activate</>
                              }
                            </button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
