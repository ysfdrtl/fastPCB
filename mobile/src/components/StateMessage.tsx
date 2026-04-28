import { StyleSheet, Text, View } from "react-native";
import { colors } from "@/theme/colors";

type StateMessageProps = {
  message: string;
  tone?: "neutral" | "error" | "success";
};

export function StateMessage({ message, tone = "neutral" }: StateMessageProps) {
  return (
    <View style={[styles.box, styles[tone]]}>
      <Text style={[styles.text, tone === "error" ? styles.errorText : null, tone === "success" ? styles.successText : null]}>{message}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  box: {
    borderRadius: 8,
    borderWidth: 1,
    padding: 12,
    backgroundColor: colors.surfaceMuted,
    borderColor: colors.border
  },
  error: {
    backgroundColor: "#fff5f5",
    borderColor: "#fecaca"
  },
  success: {
    backgroundColor: "#ecfdf3",
    borderColor: "#abefc6"
  },
  neutral: {},
  text: {
    color: colors.muted,
    fontSize: 14,
    lineHeight: 20
  },
  errorText: {
    color: colors.danger
  },
  successText: {
    color: colors.success
  }
});

