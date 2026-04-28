import { StyleSheet, Text, TextInput, type TextInputProps, View } from "react-native";
import { colors } from "@/theme/colors";

type TextFieldProps = TextInputProps & {
  label?: string;
};

export function TextField({ label, style, multiline, ...props }: TextFieldProps) {
  return (
    <View style={styles.wrap}>
      {label ? <Text style={styles.label}>{label}</Text> : null}
      <TextInput
        multiline={multiline}
        placeholderTextColor="#98a2b3"
        style={[styles.input, multiline ? styles.multiline : null, style]}
        {...props}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  wrap: {
    gap: 7
  },
  label: {
    color: colors.text,
    fontSize: 13,
    fontWeight: "700"
  },
  input: {
    minHeight: 48,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: colors.border,
    backgroundColor: colors.surface,
    color: colors.text,
    paddingHorizontal: 14,
    fontSize: 15
  },
  multiline: {
    minHeight: 104,
    paddingTop: 12,
    textAlignVertical: "top"
  }
});

