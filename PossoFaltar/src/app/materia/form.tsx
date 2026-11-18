import { router, useLocalSearchParams } from 'expo-router';
import { useState, type ReactNode } from 'react';
import { Platform, Pressable, StyleSheet, TextInput, View, type TextInputProps } from 'react-native';

import { Icon } from '@/components/icon';
import { NavHeader } from '@/components/nav-header';
import { Screen } from '@/components/screen';
import { AppText, Button, Card, Chip, IconButton, Stepper } from '@/components/ui';
import { Radius, Spacing, SubjectColors } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import { formatGrade, maxAbsencesFor, plural } from '@/lib/calc';
import { WEEKDAY_LONG, WEEKDAY_SHORT, WEEK_ORDER } from '@/lib/dates';
import { successFeedback, warningFeedback } from '@/lib/feedback';
import { newId } from '@/lib/id';
import { useStore, useSubject } from '@/lib/store';
import type { Assessment, SubjectInput, Weekday } from '@/lib/types';

const TOTAL_SUGGESTIONS = [40, 60, 80, 120];

export default function SubjectFormScreen() {
  const { id } = useLocalSearchParams<{ id?: string }>();
  const existing = useSubject(id);
  const { data, addSubject, updateSubject } = useStore();
  const theme = useTheme();
  const isEdit = !!existing;

  const [name, setName] = useState(existing?.name ?? '');
  const [professor, setProfessor] = useState(existing?.professor ?? '');
  const [color, setColor] = useState<string>(
    existing?.color ?? SubjectColors[data.subjects.length % SubjectColors.length],
  );
  const [schedule, setSchedule] = useState<Partial<Record<Weekday, number>>>(() =>
    Object.fromEntries((existing?.schedule ?? []).map((s) => [s.weekday, s.classes])),
  );
  const [totalText, setTotalText] = useState(String(existing?.totalClasses ?? 80));
  const [minAttendance, setMinAttendance] = useState(existing?.minAttendance ?? 75);
  const [passingGrade, setPassingGrade] = useState(existing?.passingGrade ?? 6);
  const [assessments, setAssessments] = useState<Assessment[]>(
    existing?.assessments ?? [
      { id: newId(), name: 'N1', weight: 1, grade: null },
      { id: newId(), name: 'N2', weight: 1, grade: null },
    ],
  );
  const [error, setError] = useState<string | null>(null);

  const totalClasses = Number.parseInt(totalText, 10) || 0;
  const maxAbsences = maxAbsencesFor({ totalClasses, minAttendance });
  const selectedDays = WEEK_ORDER.filter((wd) => schedule[wd] !== undefined);

  const toggleDay = (wd: Weekday) =>
    setSchedule((prev) => {
      const next = { ...prev };
      if (next[wd] !== undefined) delete next[wd];
      else next[wd] = 2;
      return next;
    });

  const updateAssessment = (aid: string, patch: Partial<Assessment>) =>
    setAssessments((list) => list.map((a) => (a.id === aid ? { ...a, ...patch } : a)));

  const fail = (message: string) => {
    warningFeedback();
    setError(message);
  };

  const save = () => {
    if (!name.trim()) return fail('Dê um nome para a matéria.');
    if (totalClasses <= 0) return fail('Informe o total de aulas do semestre.');
    if (selectedDays.length === 0) return fail('Escolha pelo menos um dia de aula.');

    const input: SubjectInput = {
      name: name.trim(),
      professor: professor.trim(),
      color,
      totalClasses,
      minAttendance,
      schedule: selectedDays.map((wd) => ({ weekday: wd, classes: schedule[wd] ?? 1 })),
      passingGrade,
      maxGrade: existing?.maxGrade ?? 10,
      assessments: assessments
        .filter((a) => a.name.trim())
        .map((a) => ({ ...a, name: a.name.trim() })),
    };
    if (existing) updateSubject(existing.id, input);
    else addSubject(input);
    successFeedback();
    router.back();
  };

  return (
    <Screen
      withTabBar={false}
      header={
        <NavHeader
          title={isEdit ? 'Editar matéria' : 'Nova matéria'}
          backIcon="close"
          safeTop={Platform.OS !== 'ios'}
          right={<Button title="Salvar" size="sm" color={color} onPress={save} />}
        />
      }>
      {/* Identificação */}
      <Field label="Nome da matéria">
        <Input
          value={name}
          onChangeText={(v) => {
            setName(v);
            setError(null);
          }}
          placeholder="Ex.: Desenvolvimento Mobile"
          autoFocus={!isEdit}
          returnKeyType="next"
        />
      </Field>
      <Field label="Professor(a)" hint="Opcional">
        <Input value={professor} onChangeText={setProfessor} placeholder="Ex.: Prof. Ricardo" />
      </Field>

      <Field label="Cor">
        <View style={styles.colors}>
          {SubjectColors.map((c) => (
            <Pressable
              key={c}
              accessibilityRole="button"
              accessibilityLabel={`Cor ${c}`}
              accessibilityState={{ selected: c === color }}
              onPress={() => setColor(c)}
              style={[
                styles.swatch,
                { backgroundColor: c, borderColor: c === color ? theme.text : 'transparent' },
              ]}>
              {c === color && <Icon name="check" size={16} color="#fff" />}
            </Pressable>
          ))}
        </View>
      </Field>

      {/* Frequência */}
      <Field label="Dias de aula" hint="Toque nos dias em que você tem essa matéria">
        <View style={styles.days}>
          {WEEK_ORDER.map((wd) => (
            <Chip
              key={wd}
              label={WEEKDAY_SHORT[wd]}
              selected={schedule[wd] !== undefined}
              color={color}
              onPress={() => {
                toggleDay(wd);
                setError(null);
              }}
              style={styles.dayChip}
              textStyle={styles.dayChipText}
            />
          ))}
        </View>
        {selectedDays.length > 0 && (
          <Card style={styles.daysCard}>
            {selectedDays.map((wd) => (
              <View key={wd} style={styles.dayRow}>
                <AppText variant="bodyStrong" style={styles.flex}>
                  {WEEKDAY_LONG[wd].charAt(0).toUpperCase() + WEEKDAY_LONG[wd].slice(1)}
                </AppText>
                <Stepper
                  compact
                  value={schedule[wd] ?? 1}
                  min={1}
                  max={8}
                  format={(v) => plural(v, 'aula')}
                  onChange={(v) => setSchedule((prev) => ({ ...prev, [wd]: v }))}
                />
              </View>
            ))}
          </Card>
        )}
      </Field>

      <Field
        label="Total de aulas no semestre"
        hint="Dica: costuma ser a carga horária da matéria (ex.: 80h = 80 aulas)">
        <View style={styles.totalRow}>
          <Input
            value={totalText}
            onChangeText={(v) => {
              setTotalText(v.replace(/\D/g, '').slice(0, 4));
              setError(null);
            }}
            keyboardType="number-pad"
            style={styles.totalInput}
          />
          {TOTAL_SUGGESTIONS.map((n) => (
            <Chip
              key={n}
              label={String(n)}
              selected={totalClasses === n}
              color={color}
              onPress={() => setTotalText(String(n))}
            />
          ))}
        </View>
      </Field>

      <Field label="Frequência mínima">
        <View style={styles.rowBetween}>
          <Stepper
            value={minAttendance}
            min={50}
            max={100}
            step={5}
            format={(v) => `${v}%`}
            onChange={setMinAttendance}
          />
        </View>
        <View style={[styles.preview, { backgroundColor: theme.primarySoft }]}>
          <AppText variant="bodyStrong" color="primary">
            {totalClasses > 0
              ? `Você poderá faltar até ${plural(maxAbsences, 'aula')} 🙌`
              : 'Informe o total de aulas'}
          </AppText>
        </View>
      </Field>

      {/* Notas */}
      <Field label="Avaliações" hint="Use pesos diferentes se a N2 valer mais, por exemplo">
        <Card style={styles.daysCard}>
          {assessments.map((a) => (
            <View key={a.id} style={styles.assessmentRow}>
              <Input
                value={a.name}
                onChangeText={(v) => updateAssessment(a.id, { name: v.slice(0, 16) })}
                placeholder="Nome"
                style={styles.assessmentName}
              />
              <Stepper
                compact
                value={a.weight}
                min={1}
                max={10}
                format={(v) => `peso ${v}`}
                onChange={(v) => updateAssessment(a.id, { weight: v })}
              />
              <IconButton
                icon="trash"
                size={34}
                accessibilityLabel={`Remover ${a.name}`}
                color={theme.textMuted}
                background="transparent"
                onPress={() => setAssessments((list) => list.filter((x) => x.id !== a.id))}
              />
            </View>
          ))}
          <Button
            title="Adicionar avaliação"
            icon="plus"
            variant="ghost"
            size="sm"
            color={color}
            onPress={() =>
              setAssessments((list) => [
                ...list,
                { id: newId(), name: `N${list.length + 1}`, weight: 1, grade: null },
              ])
            }
          />
        </Card>
      </Field>

      <Field label="Média para passar">
        <Stepper
          value={passingGrade}
          min={0}
          max={10}
          step={0.5}
          format={formatGrade}
          onChange={setPassingGrade}
        />
      </Field>

      {error && (
        <View style={[styles.error, { backgroundColor: theme.dangerSoft }]}>
          <Icon name="warning" size={16} color={theme.danger} />
          <AppText variant="bodyStrong" style={{ color: theme.danger }}>
            {error}
          </AppText>
        </View>
      )}

      <Button
        title={isEdit ? 'Salvar alterações' : 'Adicionar matéria'}
        icon="check"
        size="lg"
        color={color}
        style={styles.saveButton}
        onPress={save}
      />
    </Screen>
  );
}

function Field({
  label,
  hint,
  children,
}: {
  label: string;
  hint?: string;
  children: ReactNode;
}) {
  return (
    <View style={styles.field}>
      <View style={styles.fieldLabel}>
        <AppText variant="label" color="textSecondary">
          {label}
        </AppText>
        {hint && (
          <AppText variant="caption" color="textMuted">
            {hint}
          </AppText>
        )}
      </View>
      {children}
    </View>
  );
}

function Input({ style, ...props }: TextInputProps) {
  const theme = useTheme();
  return (
    <TextInput
      placeholderTextColor={theme.textMuted}
      {...props}
      style={[
        styles.input,
        { backgroundColor: theme.card, borderColor: theme.border, color: theme.text },
        style,
      ]}
    />
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1 },
  field: { gap: Spacing.two, marginTop: Spacing.five },
  fieldLabel: { gap: 2, paddingHorizontal: Spacing.one },
  input: {
    height: 50,
    borderRadius: Radius.md,
    borderWidth: 1,
    paddingHorizontal: Spacing.four,
    fontSize: 16,
    fontWeight: '600',
  },
  colors: { flexDirection: 'row', flexWrap: 'wrap', gap: Spacing.two },
  swatch: {
    width: 34,
    height: 34,
    borderRadius: 17,
    borderWidth: 3,
    alignItems: 'center',
    justifyContent: 'center',
  },
  days: { flexDirection: 'row', gap: 6 },
  dayChip: { flex: 1, paddingHorizontal: 0 },
  dayChipText: { fontSize: 13 },
  daysCard: { gap: Spacing.three, padding: Spacing.three },
  dayRow: { flexDirection: 'row', alignItems: 'center', gap: Spacing.three },
  totalRow: { flexDirection: 'row', flexWrap: 'wrap', alignItems: 'center', gap: Spacing.two },
  totalInput: { width: 80, textAlign: 'center', fontSize: 18, fontWeight: '800' },
  rowBetween: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' },
  preview: { borderRadius: Radius.md, padding: Spacing.three },
  assessmentRow: { flexDirection: 'row', alignItems: 'center', gap: Spacing.two },
  assessmentName: { flex: 1, height: 44, minWidth: 0 },
  error: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.two,
    padding: Spacing.three,
    borderRadius: Radius.md,
    marginTop: Spacing.five,
  },
  saveButton: { marginTop: Spacing.six },
});
