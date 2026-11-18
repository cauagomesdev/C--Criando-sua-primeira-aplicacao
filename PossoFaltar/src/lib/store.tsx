import AsyncStorage from '@react-native-async-storage/async-storage';
import { createContext, use, useEffect, useRef, useState, type ReactNode } from 'react';

import { newId } from './id';
import { buildSampleData } from './sample';
import type { AppData, AttendanceStatus, Subject, SubjectInput } from './types';

const STORAGE_KEY = 'posso-faltar:data:v1';
const EMPTY: AppData = { version: 1, subjects: [], records: [] };

interface Store {
  data: AppData;
  ready: boolean;
  addSubject: (input: SubjectInput) => string;
  updateSubject: (id: string, input: SubjectInput) => void;
  deleteSubject: (id: string) => void;
  setAttendance: (
    subjectId: string,
    date: string,
    status: AttendanceStatus,
    classes: number,
  ) => void;
  clearAttendance: (subjectId: string, date: string) => void;
  deleteRecord: (recordId: string) => void;
  setGrade: (subjectId: string, assessmentId: string, grade: number | null) => void;
  loadSample: () => void;
  resetAll: () => void;
}

const StoreContext = createContext<Store | null>(null);

function isAppData(value: unknown): value is AppData {
  return (
    typeof value === 'object' &&
    value !== null &&
    Array.isArray((value as AppData).subjects) &&
    Array.isArray((value as AppData).records)
  );
}

export function StoreProvider({ children }: { children: ReactNode }) {
  const [data, setData] = useState<AppData>(EMPTY);
  const [ready, setReady] = useState(false);
  const skipNextSave = useRef(true);

  // Carrega os dados salvos no aparelho.
  useEffect(() => {
    AsyncStorage.getItem(STORAGE_KEY)
      .then((raw) => {
        if (!raw) return;
        const parsed: unknown = JSON.parse(raw);
        if (isAppData(parsed)) setData(parsed);
      })
      .catch(() => {})
      .finally(() => setReady(true));
  }, []);

  // Salva a cada alteração (depois do carregamento inicial).
  useEffect(() => {
    if (!ready) return;
    if (skipNextSave.current) {
      skipNextSave.current = false;
      return;
    }
    AsyncStorage.setItem(STORAGE_KEY, JSON.stringify(data)).catch(() => {});
  }, [data, ready]);

  const update = (fn: (prev: AppData) => AppData) => setData((prev) => fn(prev));

  const store: Store = {
    data,
    ready,
    addSubject: (input) => {
      const subject: Subject = { ...input, id: newId(), createdAt: Date.now() };
      update((d) => ({ ...d, subjects: [...d.subjects, subject] }));
      return subject.id;
    },
    updateSubject: (id, input) =>
      update((d) => ({
        ...d,
        subjects: d.subjects.map((s) => (s.id === id ? { ...s, ...input } : s)),
      })),
    deleteSubject: (id) =>
      update((d) => ({
        ...d,
        subjects: d.subjects.filter((s) => s.id !== id),
        records: d.records.filter((r) => r.subjectId !== id),
      })),
    setAttendance: (subjectId, date, status, classes) =>
      update((d) => {
        const others = d.records.filter((r) => !(r.subjectId === subjectId && r.date === date));
        return {
          ...d,
          records: [...others, { id: newId(), subjectId, date, status, classes }],
        };
      }),
    clearAttendance: (subjectId, date) =>
      update((d) => ({
        ...d,
        records: d.records.filter((r) => !(r.subjectId === subjectId && r.date === date)),
      })),
    deleteRecord: (recordId) =>
      update((d) => ({ ...d, records: d.records.filter((r) => r.id !== recordId) })),
    setGrade: (subjectId, assessmentId, grade) =>
      update((d) => ({
        ...d,
        subjects: d.subjects.map((s) =>
          s.id !== subjectId
            ? s
            : {
                ...s,
                assessments: s.assessments.map((a) =>
                  a.id === assessmentId ? { ...a, grade } : a,
                ),
              },
        ),
      })),
    loadSample: () => setData(buildSampleData()),
    resetAll: () => setData(EMPTY),
  };

  return <StoreContext value={store}>{children}</StoreContext>;
}

export function useStore(): Store {
  const store = use(StoreContext);
  if (!store) throw new Error('useStore precisa estar dentro de <StoreProvider>');
  return store;
}

export function useSubject(id: string | undefined): Subject | undefined {
  const { data } = useStore();
  return data.subjects.find((s) => s.id === id);
}
