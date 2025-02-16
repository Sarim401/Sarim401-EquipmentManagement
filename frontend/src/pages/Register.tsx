import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';

const API_URL = 'http://localhost:5223/api/auth/register';

const Register: React.FC = () => {
  const [formData, setFormData] = useState({
    userName: '',
    email: '',
    password: '',
  });
  const [error, setError] = useState<string | null>(null);
  const [passwordError, setPasswordError] = useState<string | null>(null);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setPasswordError(null);

    try {
      await axios.post(API_URL, { ...formData, role: 'User' }, {
        headers: { 'Content-Type': 'application/json' },
      });
      navigate('/login');
    } catch (err: any) {
      console.error('Błąd rejestracji:', err);
      
      const errors = err.response?.data; 

      if (Array.isArray(errors)) {
        setError(errors.map((e: any) => e.description).join("\n"));

        const passwordErrors = errors.filter((e: any) => e.property === "password");
        if (passwordErrors.length > 0) {
          setPasswordError(passwordErrors.map((e: any) => e.description).join("\n"));
        }
      } else {
        setError('Rejestracja nie powiodła się');
      }
    }
  };

  return (
    <div className="flex justify-center items-center h-screen bg-gray-100">
      <div className="bg-white p-6 rounded-lg shadow-lg w-96">
        <h2 className="text-2xl font-bold mb-4 text-center">Rejestracja</h2>
        
        {}
        {error && (
          <div className="text-red-500 text-sm text-center mb-4">
            {error}
          </div>
        )}

        {}
        <form onSubmit={handleSubmit} className="space-y-4">
          <input
            type="text"
            name="userName"
            placeholder="Nazwa użytkownika"
            value={formData.userName}
            onChange={handleChange}
            className="w-full p-2 border rounded"
            required
          />
          <input
            type="email"
            name="email"
            placeholder="Email"
            value={formData.email}
            onChange={handleChange}
            className="w-full p-2 border rounded"
            required
          />
          
          {}
          {passwordError && (
            <div className="text-red-500 text-sm mt-2">
              {passwordError.split("\n").map((error, index) => (
                <p key={index}>{error}</p>
              ))}
            </div>
          )}

          <input
            type="password"
            name="password"
            placeholder="Hasło"
            value={formData.password}
            onChange={handleChange}
            className="w-full p-2 border rounded"
            required
          />

          <button type="submit" className="w-full bg-blue-500 text-white p-2 rounded">
            Zarejestruj się
          </button>
        </form>

        <p className="text-sm text-center mt-4">
          Masz już konto? <a href="/login" className="text-blue-500">Zaloguj się</a>
        </p>
      </div>
    </div>
  );
};

export default Register;
