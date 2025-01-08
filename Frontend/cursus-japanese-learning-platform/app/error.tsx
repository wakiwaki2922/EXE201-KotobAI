'use client';

import { Button } from '@/components/ui/button';
import React from 'react';

const Error: React.FC<{ error: Error, reset: () => void }> = ({ error, reset }) => {
  return (
    <div>
      <h1>500 - Server-side Error</h1>
      <p>{error.message}</p>
      <Button onClick={() => reset()}>Try again</Button>
    </div>
  );
};

export default Error;