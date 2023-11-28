import { Typography } from "@mui/material";

interface QuestionDisplayProps {
  question: string
}

function QuestionDisplay(props: QuestionDisplayProps) {

  return (
    <Typography variant="h4" component="p" mt={2} mb={2}>
      {props.question}
    </Typography>
  );
}

export default QuestionDisplay;