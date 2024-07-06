import { SwipeableDrawer } from "@mui/material";

interface AppDrawerProps {
  open: boolean,
  anchor: 'left' | 'top' | 'right' | 'bottom',
  children?: React.ReactNode,
  handleClose: () => void
}

AppDrawer.defaultProps = {
  anchor: 'top'
};

function AppDrawer(props: AppDrawerProps) {

  return (
    <SwipeableDrawer
      anchor={props.anchor}
      open={props.open}
      onOpen={props.handleClose}
      onClose={props.handleClose}
      disableBackdropTransition={true}
      disableDiscovery={true}
      disableSwipeToOpen={true}
    >
      {props.children}
    </SwipeableDrawer>
  );
}

export default AppDrawer;