export function Spinner({ size = 'md' }: { size?: 'sm' | 'md' | 'lg' }) {
  const s = { sm: 'h-4 w-4', md: 'h-8 w-8', lg: 'h-12 w-12' }[size];
  return (
    <div className={`${s} animate-spin rounded-full border-2 border-indigo-200 border-t-indigo-600`} />
  );
}

export function PageLoader() {
  return (
    <div className="flex min-h-[400px] items-center justify-center">
      <Spinner size="lg" />
    </div>
  );
}

export function SkeletonCard() {
  return (
    <div className="animate-pulse rounded-2xl bg-white p-4 shadow-md">
      <div className="mb-4 h-52 rounded-xl bg-slate-200" />
      <div className="mb-2 h-3 w-1/3 rounded bg-slate-200" />
      <div className="mb-3 h-5 w-2/3 rounded bg-slate-200" />
      <div className="h-4 w-1/4 rounded bg-slate-200" />
    </div>
  );
}
