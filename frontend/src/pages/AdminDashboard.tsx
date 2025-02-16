import React, { useState, useEffect } from 'react';
import { useMutation, useQuery } from 'react-query';
import { useNavigate } from 'react-router-dom';
import { getUsers, changeUserRole, deleteUser } from '../services/adminService'; 
import { logout } from '../services/authService';

// Definicja interfejsu User
interface User {
  id: string;
  userName: string;
  email: string;
  roles: string[]; // Rola użytkownika
}

const AdminDashboard = () => {
  const navigate = useNavigate();
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
  const [roles, setRoles] = useState<{ [key: string]: string }>({}); // Obiekt przechowujący role poszczególnych użytkowników
  const [error, setError] = useState<string | null>(null);

  // Zakładając, że masz dostęp do ID aktualnie zalogowanego użytkownika
  const loggedInUserId = "06bdabfb-dc06-4602-acf6-02427ede3e67"; // Zmień to na metodę pobierania aktualnego ID użytkownika

  // Typowanie dla useQuery
  const { data: users, isLoading, error: usersError } = useQuery<User[], Error>('users', getUsers);

  const changeRoleMutation = useMutation(changeUserRole, {
    onSuccess: () => {
      alert('Rola użytkownika została zmieniona!');
    },
    onError: (error: any) => {
      setError('Błąd podczas zmiany roli użytkownika');
    }
  });

  const deleteUserMutation = useMutation(deleteUser, {
    onSuccess: () => {
      alert('Użytkownik został usunięty!');
    },
    onError: (error: any) => {
      setError('Błąd podczas usuwania użytkownika');
    }
  });

  const handleRoleChange = (userId: string) => {
    if (userId && roles[userId]) {
      changeRoleMutation.mutate({ userId, newRole: roles[userId] });
    }
  };

  const handleDeleteUser = (userId: string) => {
    if (userId !== loggedInUserId) {
      deleteUserMutation.mutate(userId); // Wywołanie funkcji usuwania użytkownika, ale nie dla zalogowanego
    }
  };

  const handleLogout = () => {
    logout(navigate);
  };

  // Obsługa ładowania i błędów
  if (isLoading) return <p>Ładowanie użytkowników...</p>;
  if (usersError) return <p>Wystąpił błąd: {usersError.message}</p>;

  return (
    <div>
      <h1>Panel Administratora</h1>
      <div>
        <h2>Zarządzanie użytkownikami</h2>

        <ul>
          {users
            ?.filter(user => user.id !== loggedInUserId) // Filtrowanie, aby nie wyświetlać aktualnie zalogowanego użytkownika
            .map((user: User) => {
              // Ustawiamy początkową rolę z user.roles[0]
              const initialRole = user.roles[0];

              return (
                <li key={user.id}>
                  <span>{user.userName}</span>

                  {/* Ustawiamy początkową rolę z user.roles[0], ale pozwalamy na jej zmianę */}
                  <select
                    value={roles[user.id] || initialRole} // Pobieramy rolę z obiektu 'roles' dla konkretnego użytkownika
                    onChange={(e) => setRoles((prev) => ({ ...prev, [user.id]: e.target.value }))} // Aktualizujemy rolę tylko dla tego użytkownika
                  >
                    <option value="User">Użytkownik</option>
                    <option value="Admin">Administrator</option>
                  </select>

                  <button onClick={() => handleRoleChange(user.id)}>Zmień rolę</button>

                  {/* Zablokowanie opcji usuwania dla zalogowanego użytkownika */}
                  <button 
                    onClick={() => handleDeleteUser(user.id)} 
                    disabled={user.id === loggedInUserId} // Zablokowanie przycisku usuwania dla zalogowanego
                  >
                    Usuń
                  </button>
                </li>
              );
            })
          }
        </ul>

        {error && <p style={{ color: 'red' }}>{error}</p>}
      </div>

      <button onClick={handleLogout} className="logout-button">
        Wyloguj się
      </button>
    </div>
  );
};

export default AdminDashboard;
