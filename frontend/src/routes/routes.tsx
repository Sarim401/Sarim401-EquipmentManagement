import { Routes, Route } from 'react-router-dom';
import Login from '../pages/login';
import Dashboard from '../pages/Dashboard';

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/dashboard" element={<Dashboard />} />
    </Routes>
  );
};

export default AppRoutes;
