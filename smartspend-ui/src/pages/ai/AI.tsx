import { useState } from 'react';
import api from '../../api/axiosInstance';
import styles from '../../styles/AI.module.css';

const AI = () => {
  const [parseText, setParseText] = useState('');
  const [parseResult, setParseResult] = useState<any>(null);

  const [reportMonth, setReportMonth] = useState(new Date().getMonth() + 1);
  const [reportYear, setReportYear] = useState(new Date().getFullYear());
  const [reportResult, setReportResult] = useState('');

  const [tipsResult, setTipsResult] = useState('');

  const [investMonth, setInvestMonth] = useState(new Date().getMonth() + 1);
  const [investYear, setInvestYear] = useState(new Date().getFullYear());
  const [investResult, setInvestResult] = useState('');

  const [loading, setLoading] = useState<string | null>(null);
  const [error, setError] = useState('');

  const handleParseExpense = async () => {
    if (!parseText) return;
    setLoading('parse');
    setError('');
    try {
      const res = await api.post('/api/ai/parse-expense', { text: parseText });
      setParseResult(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'AI request failed.');
    } finally {
      setLoading(null);
    }
  };

  const handleMonthlyReport = async () => {
    setLoading('report');
    setError('');
    try {
      const res = await api.post('/api/ai/monthly-report', { month: reportMonth, year: reportYear });
      setReportResult(res.data.result);
    } catch (err: any) {
      setError(err.response?.data?.message || 'AI request failed.');
    } finally {
      setLoading(null);
    }
  };

  const handleTips = async () => {
    setLoading('tips');
    setError('');
    try {
      const res = await api.post('/api/ai/tips');
      setTipsResult(res.data.result);
    } catch (err: any) {
      setError(err.response?.data?.message || 'AI request failed.');
    } finally {
      setLoading(null);
    }
  };

  const handleInvestmentAdvice = async () => {
    setLoading('invest');
    setError('');
    try {
      const res = await api.post('/api/ai/investment-advice', { month: investMonth, year: investYear });
      setInvestResult(res.data.result);
    } catch (err: any) {
      setError(err.response?.data?.message || 'AI request failed.');
    } finally {
      setLoading(null);
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>🤖 AI Insights</h1>
      {error && <p className={styles.error}>{error}</p>}

      <div className={styles.grid}>
        {/* Parse Expense */}
        <div className={styles.card}>
          <h2 className={styles.cardTitle}>Natural Language Expense</h2>
          <p className={styles.cardDesc}>Type an expense in plain text and AI will parse it.</p>
          <input
            className={styles.input}
            value={parseText}
            onChange={e => setParseText(e.target.value)}
            placeholder="e.g. spent 200 on lunch today"
          />
          <button className={styles.btn} onClick={handleParseExpense} disabled={loading === 'parse'}>
            {loading === 'parse' ? 'Parsing...' : 'Parse Expense'}
          </button>
          {parseResult && (
            <div className={styles.result}>
              <p><strong>Title:</strong> {parseResult.title}</p>
              <p><strong>Amount:</strong> ₹{parseResult.amount}</p>
              <p><strong>Category:</strong> {parseResult.categoryName}</p>
              {parseResult.note && <p><strong>Note:</strong> {parseResult.note}</p>}
            </div>
          )}
        </div>

        {/* Monthly Report */}
        <div className={styles.card}>
          <h2 className={styles.cardTitle}>Monthly Report</h2>
          <p className={styles.cardDesc}>Get an AI-generated summary of your monthly spending.</p>
          <div className={styles.row}>
            <select value={reportMonth} onChange={e => setReportMonth(parseInt(e.target.value))}>
              {Array.from({ length: 12 }, (_, i) => (
                <option key={i + 1} value={i + 1}>
                  {new Date(2000, i).toLocaleString('default', { month: 'long' })}
                </option>
              ))}
            </select>
            <input type="number" value={reportYear} onChange={e => setReportYear(parseInt(e.target.value))} />
          </div>
          <button className={styles.btn} onClick={handleMonthlyReport} disabled={loading === 'report'}>
            {loading === 'report' ? 'Generating...' : 'Generate Report'}
          </button>
          {reportResult && <div className={styles.result}><p>{reportResult}</p></div>}
        </div>

        {/* Spending Tips */}
        <div className={styles.card}>
          <h2 className={styles.cardTitle}>Spending Tips</h2>
          <p className={styles.cardDesc}>Get personalized tips based on your recent spending.</p>
          <button className={styles.btn} onClick={handleTips} disabled={loading === 'tips'}>
            {loading === 'tips' ? 'Loading...' : 'Get Tips'}
          </button>
          {tipsResult && <div className={styles.result}><p>{tipsResult}</p></div>}
        </div>

        {/* Investment Advice */}
        <div className={styles.card}>
          <h2 className={styles.cardTitle}>SIP Investment Advice</h2>
          <p className={styles.cardDesc}>Get India-specific SIP recommendations based on your budget.</p>
          <div className={styles.row}>
            <select value={investMonth} onChange={e => setInvestMonth(parseInt(e.target.value))}>
              {Array.from({ length: 12 }, (_, i) => (
                <option key={i + 1} value={i + 1}>
                  {new Date(2000, i).toLocaleString('default', { month: 'long' })}
                </option>
              ))}
            </select>
            <input type="number" value={investYear} onChange={e => setInvestYear(parseInt(e.target.value))} />
          </div>
          <button className={styles.btn} onClick={handleInvestmentAdvice} disabled={loading === 'invest'}>
            {loading === 'invest' ? 'Loading...' : 'Get Advice'}
          </button>
          {investResult && <div className={styles.result}><p>{investResult}</p></div>}
        </div>
      </div>
    </div>
  );
};

export default AI;