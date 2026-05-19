import { useEffect, useState } from 'react';
import api from '../../api/axiosInstance';
import { Expense, Category } from '../../types/index';
import styles from '../../styles/Expenses.module.css';
import Toast from '../../components/common/Toast';

const Expenses = () => {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingExpense, setEditingExpense] = useState<Expense | null>(null);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' | 'info' } | null>(null);

  const [title, setTitle] = useState('');
  const [amount, setAmount] = useState('');
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
  const [note, setNote] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [expensesRes, categoriesRes] = await Promise.all([
        api.get<Expense[]>('/api/expenses'),
        api.get<Category[]>('/api/categories')
      ]);
      setExpenses(expensesRes.data);
      setCategories(categoriesRes.data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const resetForm = () => {
    setTitle('');
    setAmount('');
    setDate(new Date().toISOString().split('T')[0]);
    setNote('');
    setCategoryId('');
    setError('');
    setEditingExpense(null);
    setShowForm(false);
  };

  const handleEdit = (expense: Expense) => {
    setEditingExpense(expense);
    setTitle(expense.title);
    setAmount(expense.amount.toString());
    setDate(new Date(expense.date).toISOString().split('T')[0]);
    setNote(expense.note || '');
    setCategoryId(expense.categoryId.toString());
    setShowForm(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    const payload = {
      title,
      amount: parseFloat(amount),
      date: new Date(date).toISOString(),
      note,
      categoryId: parseInt(categoryId)
    };

    try {
      if (editingExpense) {
        await api.put(`/api/expenses/${editingExpense.id}`, payload);
      } else {
        await api.post('/api/expenses', payload);
      }
      await fetchData();
      setToast({ message: editingExpense ? 'Expense updated!' : 'Expense added!', type: 'success' });
      resetForm();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Something went wrong.');
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Delete this expense?')) return;
    try {
      await api.delete(`/api/expenses/${id}`);
      await fetchData();
      setToast({ message: 'Expense deleted!', type: 'success' });
    } catch (err) {
      console.error(err);
    }
  };

  if (loading) return <div className={styles.loading}>Loading...</div>;

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Expenses</h1>
        <button className={styles.addBtn} onClick={() => setShowForm(true)}>
          + Add Expense
        </button>
      </div>

      {showForm && (
        <div className={styles.formCard}>
          <h2>{editingExpense ? 'Edit Expense' : 'Add Expense'}</h2>
          {error && <p className={styles.error}>{error}</p>}
          <form onSubmit={handleSubmit} className={styles.form}>
            <div className={styles.formGrid}>
              <div className={styles.field}>
                <label>Title</label>
                <input value={title} onChange={e => setTitle(e.target.value)} placeholder="Expense title" required />
              </div>
              <div className={styles.field}>
                <label>Amount (₹)</label>
                <input type="number" value={amount} onChange={e => setAmount(e.target.value)} placeholder="0.00" required />
              </div>
              <div className={styles.field}>
                <label>Date</label>
                <input type="date" value={date} onChange={e => setDate(e.target.value)} required />
              </div>
              <div className={styles.field}>
                <label>Category</label>
                <select value={categoryId} onChange={e => setCategoryId(e.target.value)} required>
                  <option value="">Select category</option>
                  {categories.map(c => (
                    <option key={c.id} value={c.id}>{c.icon} {c.name}</option>
                  ))}
                </select>
              </div>
              <div className={styles.field}>
                <label>Note (optional)</label>
                <input value={note} onChange={e => setNote(e.target.value)} placeholder="Optional note" />
              </div>
            </div>
            <div className={styles.formActions}>
              <button type="submit" className={styles.saveBtn}>
                {editingExpense ? 'Update' : 'Save'}
              </button>
              <button type="button" className={styles.cancelBtn} onClick={resetForm}>
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {expenses.length === 0 ? (
        <p className={styles.empty}>No expenses yet. Start adding some!</p>
      ) : (
        <div className={styles.table}>
          <div className={styles.tableHeader}>
            <span>Title</span>
            <span>Category</span>
            <span>Date</span>
            <span>Amount</span>
            <span>Actions</span>
          </div>
          {expenses.map(expense => (
            <div key={expense.id} className={styles.tableRow}>
              <span>{expense.title}</span>
              <span>{expense.categoryIcon} {expense.categoryName}</span>
              <span>{new Date(expense.date).toLocaleDateString()}</span>
              <span>₹{expense.amount.toFixed(2)}</span>
              <span className={styles.actions}>
                <button className={styles.editBtn} onClick={() => handleEdit(expense)}>Edit</button>
                <button className={styles.deleteBtn} onClick={() => handleDelete(expense.id)}>Delete</button>
              </span>
            </div>
          ))}
        </div>
      )}

      {toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}

    </div>
  );
};

export default Expenses;