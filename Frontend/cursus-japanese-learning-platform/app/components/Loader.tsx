import { useEffect } from 'react'

export default function Loader() {
  useEffect(() => {
    async function getLoader() {
      const { jelly } = await import('ldrs')
      jelly.register()
    }
    getLoader()
  }, [])
  return <l-jelly color="rgb(6 83 142)"></l-jelly>
}