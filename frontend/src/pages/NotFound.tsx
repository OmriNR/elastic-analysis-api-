import { Link } from 'react-router-dom';
import { Button } from '../components/ui/Button';

export function NotFound() {
  return (
    <div className="flex min-h-[70vh] flex-col items-center justify-center gap-6 text-center px-4">
      <div className="text-8xl font-extrabold text-slate-200">404</div>
      <div>
        <h1 className="text-2xl font-bold text-slate-800">Page not found</h1>
        <p className="mt-1 text-slate-500">The page you're looking for doesn't exist.</p>
      </div>
      <Link to="/">
        <Button size="lg">Go Home</Button>
      </Link>
    </div>
  );
}
