import CloseIcon from '@mui/icons-material/Close';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, IconButton, Typography} from "@mui/material";
import React from 'react';

interface RateLimitDialogProps {
  open: boolean
}

function RateLimitDialog(props: RateLimitDialogProps) {
  const [dismissed, setDismissed] = React.useState(false);

  const handleClose = () => {
    setDismissed(true);
  };

  return (
    <Dialog
      onClose={handleClose}
      aria-labelledby="ratelimit-dialog-title"
      open={props.open && !dismissed}
    >
      <DialogTitle sx={{ m: 0, p: 2 }} id="ratelimit-dialog-title">
        429 Rate Limited
      </DialogTitle>
      <IconButton
        aria-label="close"
        onClick={handleClose}
        sx={{
          position: 'absolute',
          right: 8,
          top: 8,
          color: (theme) => theme.palette.grey[500],
        }}
      >
        <CloseIcon />
      </IconButton>
      <DialogContent dividers>
        <Typography gutterBottom>
          Wow, you must really like this app. That's great!
        </Typography>
        <Typography gutterBottom>
          You've reached the maximum number of daily questions, but come back in 12 hours if you'd like more.
        </Typography>
        <Typography gutterBottom>
          [To request an increase to this limit, contact Amber.]
        </Typography>
      </DialogContent>
      <DialogActions>
        <Button autoFocus onClick={handleClose}>
          OK
        </Button>
      </DialogActions>
    </Dialog>
  );
}

export default RateLimitDialog;