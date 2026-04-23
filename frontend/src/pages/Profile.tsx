import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { User, MapPin, Save, LogOut } from 'lucide-react';
import { updateUserProperties } from '../api/users';
import { useAuthStore } from '../store/authStore';
import { Button } from '../components/ui/Button';

export function Profile() {
  const { user, userProps, setUserProps, logout } = useAuthStore();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [userName, setUserName] = useState(userProps?.user_name ?? '');
  const [age, setAge] = useState(String(userProps?.age ?? ''));
  const [gender, setGender] = useState(userProps?.gender ?? '');
  const [city, setCity] = useState(userProps?.location?.city ?? '');
  const [country, setCountry] = useState(userProps?.location?.country ?? '');
  const [address, setAddress] = useState(userProps?.location?.address ?? '');
  const [zipCode, setZipCode] = useState(userProps?.location?.zip_code ?? '');
  const [saved, setSaved] = useState(false);

  if (!user) {
    navigate('/login');
    return null;
  }

  const { mutate, isPending } = useMutation({
    mutationFn: () =>
      updateUserProperties({
        user_id: user.user_id,
        user_name: userName,
        age: parseInt(age, 10),
        gender,
        location: { city, country, address, zip_code: zipCode },
        created_at: userProps?.created_at ?? new Date().toISOString(),
      }),
    onSuccess: (props) => {
      setUserProps(props);
      queryClient.invalidateQueries({ queryKey: ['userProps', user.user_id] });
      setSaved(true);
      setTimeout(() => setSaved(false), 2500);
    },
  });

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <div className="min-h-screen bg-slate-50 py-8">
      <div className="mx-auto max-w-2xl px-4 sm:px-6 lg:px-8">
        <h1 className="mb-6 text-3xl font-bold text-slate-900">My Profile</h1>

        {/* Account info */}
        <div className="mb-5 rounded-2xl bg-white p-6 shadow-sm">
          <div className="flex items-center gap-4">
            <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-indigo-100">
              <User className="h-8 w-8 text-indigo-600" />
            </div>
            <div>
              <p className="text-lg font-semibold text-slate-800">{userProps?.user_name || 'User'}</p>
              <p className="text-sm text-slate-500">{user.email}</p>
              <div className="mt-1 flex gap-1.5">
                {user.is_admin && (
                  <span className="rounded-full bg-purple-100 px-2.5 py-0.5 text-xs font-medium text-purple-700">Admin</span>
                )}
                <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${user.is_active ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
                  {user.is_active ? 'Active' : 'Inactive'}
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Edit profile */}
        <div className="rounded-2xl bg-white p-6 shadow-sm">
          <h2 className="mb-5 flex items-center gap-2 font-semibold text-slate-800">
            <User className="h-4 w-4 text-indigo-500" /> Personal Information
          </h2>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="mb-1.5 block text-sm font-medium text-slate-700">Name</label>
                <input
                  value={userName}
                  onChange={e => setUserName(e.target.value)}
                  className="w-full rounded-xl border border-slate-200 px-4 py-2.5 text-sm outline-none transition focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
                />
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium text-slate-700">Age</label>
                <input
                  type="number"
                  value={age}
                  onChange={e => setAge(e.target.value)}
                  className="w-full rounded-xl border border-slate-200 px-4 py-2.5 text-sm outline-none transition focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
                />
              </div>
            </div>
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-700">Gender</label>
              <select
                value={gender}
                onChange={e => setGender(e.target.value)}
                className="w-full rounded-xl border border-slate-200 px-4 py-2.5 text-sm outline-none transition focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
              >
                <option value="">Select gender</option>
                <option value="male">Male</option>
                <option value="female">Female</option>
                <option value="other">Other</option>
                <option value="prefer_not_to_say">Prefer not to say</option>
              </select>
            </div>
          </div>

          <h2 className="mb-4 mt-6 flex items-center gap-2 font-semibold text-slate-800">
            <MapPin className="h-4 w-4 text-indigo-500" /> Location
          </h2>
          <div className="space-y-4">
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-700">Address</label>
              <input
                value={address}
                onChange={e => setAddress(e.target.value)}
                className="w-full rounded-xl border border-slate-200 px-4 py-2.5 text-sm outline-none transition focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
              />
            </div>
            <div className="grid grid-cols-3 gap-4">
              <div>
                <label className="mb-1.5 block text-sm font-medium text-slate-700">City</label>
                <input
                  value={city}
                  onChange={e => setCity(e.target.value)}
                  className="w-full rounded-xl border border-slate-200 px-4 py-2.5 text-sm outline-none transition focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
                />
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium text-slate-700">Country</label>
                <input
                  value={country}
                  onChange={e => setCountry(e.target.value)}
                  className="w-full rounded-xl border border-slate-200 px-4 py-2.5 text-sm outline-none transition focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
                />
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium text-slate-700">ZIP</label>
                <input
                  value={zipCode}
                  onChange={e => setZipCode(e.target.value)}
                  className="w-full rounded-xl border border-slate-200 px-4 py-2.5 text-sm outline-none transition focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100"
                />
              </div>
            </div>
          </div>

          <div className="mt-6 flex gap-3">
            <Button
              onClick={() => mutate()}
              loading={isPending}
              className={saved ? 'bg-green-500 hover:bg-green-600' : ''}
            >
              <Save className="h-4 w-4" />
              {saved ? 'Saved!' : 'Save Changes'}
            </Button>
            <Button variant="danger" onClick={handleLogout}>
              <LogOut className="h-4 w-4" /> Sign Out
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
