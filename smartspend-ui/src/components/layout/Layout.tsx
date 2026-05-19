import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import styles from '../../styles/Layout.module.css';

const Layout = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className={styles.container}>
      <aside className={styles.sidebar}>
        <div className={styles.logo}>
          <h2>💰 SmartSpend</h2>
        </div>

        <nav className={styles.nav}>
          <NavLink to="/" end className={({ isActive }) => isActive ? styles.activeLink : styles.link}>
            📊 Dashboard
          </NavLink>
          <NavLink to="/expenses" className={({ isActive }) => isActive ? styles.activeLink : styles.link}>
            💸 Expenses
          </NavLink>
          <NavLink to="/budgets" className={({ isActive }) => isActive ? styles.activeLink : styles.link}>
            🎯 Budgets
          </NavLink>
          <NavLink to="/analytics" className={({ isActive }) => isActive ? styles.activeLink : styles.link}>
            📈 Analytics
          </NavLink>
          <NavLink to="/ai" className={({ isActive }) => isActive ? styles.activeLink : styles.link}>
            🤖 AI Insights
          </NavLink>
        </nav>

        <div className={styles.userSection}>
          <p className={styles.username}>👤 {user?.username}</p>
          <button className={styles.logoutBtn} onClick={handleLogout}>
            Logout
          </button>
        </div>
      </aside>

      <main className={styles.main}>
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;