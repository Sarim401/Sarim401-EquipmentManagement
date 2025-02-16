import React, { useState } from 'react';
import { useMutation } from 'react-query';
import { login } from '../services/authService';
import { useNavigate } from 'react-router-dom';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const loginMutation = useMutation({
    mutationFn: async () => {
      try {
        const data = await login(email, password);
        return data;
      } catch (error) {
        throw new Error('Błąd logowania');
      }
    },
    onSuccess: () => {
      console.log('Zalogowano pomyślnie');
      navigate('/dashboard');
    },
    onError: (error: any) => {
      console.error('Błąd logowania', error);
      setError('Nie udało się zalogować. Sprawdź swoje dane logowania.');
    }
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    loginMutation.mutate();
  };

  return (
    <div>
      <form onSubmit={handleSubmit}>
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <input
          type="password"
          placeholder="Hasło"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <button type="submit" disabled={loginMutation.isLoading}>Zaloguj</button>
      </form>

      {error && <p style={{ color: 'red' }}>{error}</p>}
      {loginMutation.isLoading && <p>Trwa logowanie...</p>}
    </div>
  );
};

export default Login;
