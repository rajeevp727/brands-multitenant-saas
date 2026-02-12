import { createContext, useContext } from 'react';

export type ConstantsMap = Record<string, string>;

export const AppConstantsContext = createContext<{
    constants: ConstantsMap;
    loading: boolean;
    get: (key: string, distinctDefault?: string) => string;
}>({
    constants: {},
    loading: true,
    get: () => '',
});

export const useAppConstants = () => useContext(AppConstantsContext);
