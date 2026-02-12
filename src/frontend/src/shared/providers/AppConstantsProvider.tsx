import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { AppConstantsContext } from './AppConstantsContext';
import type { ConstantsMap } from './AppConstantsContext';

export const AppConstantsProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [constants, setConstants] = useState<ConstantsMap>({});
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchConstants = async () => {
            try {
                const response = await api.get('/constants');
                setConstants(response.data);
            } catch (error) {
                console.error('Failed to fetch app constants', error);
            } finally {
                setLoading(false);
            }
        };

        fetchConstants();
    }, []);

    const get = (key: string, distinctDefault: string = '') => {
        return constants[key] || distinctDefault || key; // Return key as fallback if no default
    };

    return (
        <AppConstantsContext.Provider value={{ constants, loading, get }}>
            {children}
        </AppConstantsContext.Provider>
    );
};
