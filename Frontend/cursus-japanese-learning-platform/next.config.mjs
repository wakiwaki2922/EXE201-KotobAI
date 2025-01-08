/** @type {import('next').NextConfig} */
const nextConfig = {
    env: {
        BASE_API_URL: process.env.BASE_API_URL,
        BASE_API_URL_FE: process.env.BASE_API_URL_FE,
        PAYPAL_CLIENT_ID: process.env.PAYPAL_CLIENT_ID,
      },
    images: {
      remotePatterns: [
        {
          protocol: "https",
          hostname: "cursus-japanese-cloud-bucket.s3.ap-southeast-1.amazonaws.com",
        },
        {
          protocol: "https",
          hostname: "lh3.googleusercontent.com",
        }
      ],
    },
};

export default nextConfig;
