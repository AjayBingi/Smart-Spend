import { useEffect } from 'react';
import styles from '../../styles/Toast.module.css';

interface ToastProps {
    message: string;
    type: 'success' | 'error' | 'info';
    onClose: () => void;
}

const Toast = ({ message, type, onClose }: ToastProps) => {
    useEffect(() => {
        const timer = setTimeout(() => {
            onClose();
        }, 3000);
        return () => clearTimeout(timer);
    }, [onClose]);

    return (
        <div className={`${styles.toast} ${styles[type]}`}>
            <span>{message}</span>
            <button className={styles.close} onClick={onClose}>×</button>
        </div>
    );
};

export default Toast;